using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CatlikeCoding.SDFToolkit;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class DamageAndAge : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown damageAgeType;
    [SerializeField] private List<Material> damageAgeTypeMats;
    [SerializeField] private TMP_Dropdown noiseModifier;
    [SerializeField] private TMP_InputField noiseStrength;
    [SerializeField] private TMP_InputField vectorModX;
    [SerializeField] private TMP_InputField vectorModY;
    [SerializeField] private TMP_InputField helperLabel;
    [SerializeField] private Material textureMaskMat;
    [SerializeField] private Material textureMaskMat_AlphaOnly;
    [SerializeField] private TMP_InputField noiseScaleIF;
    [SerializeField] private TMP_InputField damageImpactStrength;
    [SerializeField] private float defaultNoiseScale = 15;
    
    /*
     * Attempts to create a damage and/or aging effect to the currently selected objects.
     */
    public void Execute(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        int width = 1024, height = 1024;
        foreach (var selected in currentlySelected)
        {  
            
            // 1. Find out if we are going to use any other objects as helpers (i.e. a window)
            List<GameObject> sampleSourceObjects = GetSampleSourceObjects(helperLabel.text);
            Texture2D tex = CreateSingleColorTexture2D(width, height, TextureFormat.Alpha8, false, new Color(0,0,0,0) );
            if (sampleSourceObjects.Count > 0)
            {
                foreach (var sampleSourceObject in sampleSourceObjects)
                {
                    Texture2D tempTex = CreateTextureMaskBase(selected, sampleSourceObject, width, height);
                    // 1.5 Apply angle modification
                    tempTex = ApplyAngleModifier(tempTex, width, height);
                    tex = CopyPixelsIntoTex2D(tex, tempTex, height, width);
                }
            }
            else
            {
                // If a source helper object is not being used, simply fill the texture with white
                tex = CreateSingleColorTexture2D(width, height, TextureFormat.RGB24, false, new Color(1,1,1,0));
            }
            
            // 2. Once we have the texture to operate on, start by expanding it (signed distance field)
            tex = ApplySDFToTexture(tex, width, height);
            // 3. Apply random noise to it
            tex = RandomNoise(tex);
            // 4. Texture mask that shit
            ApplyTextureMaskingMaterial(selected);
        }
    }

    Texture2D CopyPixelsIntoTex2D(Texture2D target, Texture2D copySource, int height, int width)
    {
        // We don't want to overwrite the old pixels in target
        Color[] pixelsToMark = new Color[height*width];
        Color[] copySourcePixels = copySource.GetPixels();
        Color[] targetPixels = target.GetPixels();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixelsToMark[y * height + x] += (copySourcePixels[y * height + x] + targetPixels[y * height + x]);
            }
        }
        target.SetPixels(pixelsToMark);
        return target;
    }

    /*
     * Add pixels as if a square was moving along the direction vector, only to new pixels covered by said square
     * Get the input vector, start on the edges being pointed to by vector
     * Then for each iteration, move vector value pixel amount,
     * once along X axis, incrementing by Y value,
     * once along Y axis, incrementing by X value
     */
    private Texture2D ApplyAngleModifier(Texture2D tex, int width, int height)
    {
        var angle = new Vector2(float.Parse(vectorModX.text != "" ? vectorModX.text : "0" , CultureInfo.InvariantCulture), float.Parse(vectorModY.text != "" ? vectorModY.text : "0", CultureInfo.InvariantCulture ));
        if (angle.magnitude <= float.Epsilon ) return tex;
        var largestSmallest = LargestSmallestOfPicture(tex);
        Vector2 startXY = FindStartXY(angle, tex, largestSmallest);
        var largest = largestSmallest.Item1;
        var smallest = largestSmallest.Item2;

        Vector2 ranges = new Vector2(Mathf.FloorToInt(Mathf.Abs(largest.x - smallest.x)), Mathf.FloorToInt(Mathf.Abs(largest.y - smallest.y)));
        
        Texture2D newDirTex = CreateSingleColorTexture2D(width, height, TextureFormat.Alpha8, false, new Color(0,0,0,0));
        Vector2 currentPixel = new Vector2 (Mathf.FloorToInt(smallest.x),  Mathf.FloorToInt(startXY.y));
        // Fill along X
        Color[] newDirTexPixels = new Color[height * width];
        if (Math.Abs(angle.y) > 0)
        {
            for (int y = 0; y < ranges.y; y++)
            {   
                for (int x = 0; x < ranges.x; x++)
                {   
                    if (currentPixel.x >= width || currentPixel.x < 0) continue;
                    if (currentPixel.y >= height || currentPixel.y < 0) continue;
                    newDirTexPixels[Mathf.FloorToInt(currentPixel.x) + height * Mathf.FloorToInt(currentPixel.y)] = Color.white;
                    currentPixel.x += 1;
                }
                currentPixel.y += angle.y;
                currentPixel.x = smallest.x + angle.x * y;
            }
        }

        currentPixel = new Vector2 (Mathf.FloorToInt(startXY.x),  Mathf.FloorToInt(smallest.y));
        // Fill along Y
        if (Math.Abs(angle.x) > 0)
        {
            for (int x = 0; x < ranges.x; x++)
            {   
                for (int y = 0; y < ranges.y; y++)
                {   
                    if (currentPixel.x >= width || currentPixel.x < 0) continue;
                    if (currentPixel.y >= height || currentPixel.y < 0) continue;
                    newDirTexPixels[Mathf.FloorToInt(currentPixel.x) + height * Mathf.FloorToInt(currentPixel.y)] = Color.white;
                    currentPixel.y += 1;
                }
                currentPixel.x += angle.x;
                currentPixel.y = smallest.y + angle.y * x;
            }
        }
        newDirTex.SetPixels(newDirTexPixels);
        newDirTex.Apply();
        DrawTextureIntoImage(newDirTex, "directionTextureTest");
        return newDirTex;
    }

    private Vector2 FindStartXY(Vector2 angle, Texture2D tex, Tuple<Vector2, Vector2> largestSmallest)
    {
        Vector2 resultAngle;
        
        bool topSide = angle.y < 0;
        bool leftSide = angle.x < 0;

        var largest = largestSmallest.Item1;
        var smallest = largestSmallest.Item2;
        if (topSide)
        {
            resultAngle = leftSide ? smallest : new Vector2(largest.x, smallest.y);
        }
        else
        {
            resultAngle = leftSide ? new Vector2(smallest.x, largest.y) : largest;
        }

        return resultAngle;
    }

    private Tuple<Vector2, Vector2> LargestSmallestOfPicture(Texture2D tex)
    {
        var texPixels = tex.GetPixels();
        Vector2 largest = Vector2.negativeInfinity; Vector2 smallest = Vector2.positiveInfinity;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (texPixels[x + tex.width * y].a > 0)
                {
                    if (x > largest.x)
                    {
                        largest.x = x;
                    }

                    if (x < smallest.x)
                    {
                        smallest.x = x;
                    }

                    if (y > largest.y)
                    {
                        largest.y = y;
                    }

                    if (y < smallest.y)
                    {
                        smallest.y = y;
                    }
                }
            }
        }

        return Tuple.Create(largest, smallest);
    }

    private void ApplyTextureMaskingMaterial(GameObject selected)
    {
        // Find out what kind of texture we working with? Like, rust or impact or smth? 
        Material dropdownMat = damageAgeTypeMats[damageAgeType.value-1];
        Material maskMat = (dropdownMat.GetInt("_AlphaCutoffEnable") != 1) ? textureMaskMat : textureMaskMat_AlphaOnly;
        var dmgImpactStrength = float.Parse(damageImpactStrength.text != "" ? damageImpactStrength.text : "1", CultureInfo.InvariantCulture);
        // Get the material used by the selected object
        var currMat = selected.GetComponent<MeshRenderer>().material;
        // Give the material the correct original texture & color (i don't think you can just replace the material cuz the shader used for the masking material is unique)
        maskMat.SetTexture("_OriginalTexture", currMat.mainTexture);
        maskMat.color = currMat.color;
        maskMat.SetFloat("_DamageImpactStrength", dmgImpactStrength);
        
        maskMat.SetTexture("_TextureToApply", dropdownMat.mainTexture);

        selected.GetComponent<MeshRenderer>().material = maskMat;
    }

    private Texture2D RandomNoise(Texture2D tex)
    {
        // First check what kind of noise we want (or if we want any at all)
        var noise = noiseModifier.options[noiseModifier.value].text;
        var scale = float.Parse(noiseScaleIF.text != "" ? noiseScaleIF.text : defaultNoiseScale.ToString(), CultureInfo.InvariantCulture);
        var strength = float.Parse(noiseStrength.text != "" ? noiseStrength.text : "1", CultureInfo.InvariantCulture);
        switch (noise)
        {
            case "Perlin":
                tex = NoiseGenerator.ApplyPerlinNoiseToTexture(tex, scale, strength);
                DrawTextureIntoImage(tex, "image3");
                break;
            case "Simplex":
                Debug.Log("simplex (NOT YET IMPLEMENTED)");
                throw new NotImplementedException();
                break;
            case "Value":
                tex = NoiseGenerator.ApplyNoiseToTexture(tex, scale, strength);
                DrawTextureIntoImage(tex, "image3");

                Debug.Log("Applying value noise");
                break;
            case "None":
                Debug.Log("No noise selected.");
                DrawTextureIntoImage(tex, "image3");
                break;
            default:
                Debug.Log("yo mama");
                break;
        }

        return tex;
        // Then apply that noise to the pixels with strength depending on the input alpha, I guess?
    }

    private Texture2D ApplySDFToTexture(Texture2D tex, int width, int height)
    {
        Texture2D dest = new Texture2D(tex.width, tex.height, TextureFormat.Alpha8, false);
        dest.alphaIsTransparency = true;
        int spreadDivisor = 16;  // Determines how far out the SDF will "grow". e.g. img size of 128 pixels and spreadDivisor of 4 => approx 30 pixels spread.
        SDFTextureGenerator.Generate(tex, dest, 0, (int)(width/spreadDivisor), (int)(height/spreadDivisor), RGBFillMode.Source);
        dest.Apply();
        DrawTextureIntoImage(dest, "image2");
        return dest;
    }

    private Texture2D CreateTextureMaskBase(GameObject selectedObject, GameObject sampleHelperObj, int width, int height)
    {
        // 1. Set up texture
        var helpLabelText = helperLabel.text;
        if (helpLabelText != "")
        {
            // Create a new background of black (representing nothing), with alpha channel also set to 0 (needed to fill SDF texture later)
            Texture2D backgroundTex = CreateSingleColorTexture2D(width, height, TextureFormat.Alpha8, false, new Color(0,0,0,0));
            // There is probably something to sample. Check if anything has the correct label
            
            var stepDist = selectedObject.GetComponent<Shape>().SizeExent / new Vector2(width, height);
            
            // Get the start position of where you should be stepping in world space (it makes sense)
            Vector2 currStep = new Vector2(selectedObject.transform.position.x - selectedObject.GetComponent<Shape>().SizeExent.x/2, selectedObject.transform.position.y - selectedObject.GetComponent<Shape>().SizeExent.y/2);
            Color[] pixelsToMark = new Color[height*width];
            RaycastHit[] hitResults = new RaycastHit[8];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    // Shoot out a ray from each point of the thing in the negative Z, towards camera. if we collide with one of the allowed objs, mark the pixel 
                    int hits = Physics.RaycastNonAlloc(new Vector3(currStep.x, currStep.y, 1), -Vector3.forward, hitResults, 10);
                    for (int i = 0; i < hits; i++)
                    {
                        if (sampleHelperObj == hitResults[i].transform.gameObject)
                        {
                            // Set the color to white to begin a mask of where windows are
                            pixelsToMark[height * y + x] = Color.white;
                            break;
                        }
                    }
                    currStep.x += stepDist.x;
                }
                // start on next row
                currStep.x = selectedObject.transform.position.x - selectedObject.GetComponent<Shape>().SizeExent.x/2; 
                currStep.y += stepDist.y;
            }
            backgroundTex.SetPixels(pixelsToMark);
            // Save texture to disk (maybe not necessary in the end, but really good for debugging purposes)
            backgroundTex.Apply();
            return backgroundTex;
        }
        else
        {
            // Everything is valid, so just use a fully filled in mask - a white texture
            return CreateSingleColorTexture2D(width, height, TextureFormat.RGB24, false, new Color(1,1,1,0));
        }
    }

    private List<GameObject> GetSampleSourceObjects(string helpLabelText)
    {
        // Start with the root
        GameObject root = GameObject.FindWithTag("Root");
        List<GameObject> foundObjects = new List<GameObject>();
        var rootChildren = root.GetComponentsInChildren<Shape>();
        foreach (var rootChild in rootChildren)
        {
            var labels = rootChild.GetComponent<Shape>()?.Labels;
            if (labels is null)
            {
                Debug.Log("There are no labels associated with the current object, skipping");
            }
            else
            {
                foreach (var label in labels)
                {
                    if (label.Equals(helpLabelText))
                    {   
                        foundObjects.Add(rootChild.gameObject);
                    }
                }
            }
        }

        return foundObjects;
    }

    // Important note: When creating a starting out texture for SDFs, remember to change the alpha channel as well! 
    private Texture2D CreateSingleColorTexture2D(int width, int height, TextureFormat textureFormat, bool mipChain,
        Color color)
    {
        var texture = new Texture2D(width, height, textureFormat, mipChain);
        // Set colors alpha value to be 1 if black 0 if white?? yezzzzz
        Color[] pixels = Enumerable.Repeat(color, width * height).ToArray();
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void DrawTextureIntoImage(Texture2D tex, string imgName)
    {
        byte[] bytes = tex.EncodeToPNG();
        var dirPath = Application.dataPath + "/SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + imgName + ".png", bytes);
        AssetDatabase.Refresh();                                            // Force asset database to refresh
    }

    
}