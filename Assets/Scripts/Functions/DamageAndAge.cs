using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatlikeCoding.SDFToolkit;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DamageAndAge : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown damageAgeType;
    [SerializeField] private TMP_Dropdown noiseModifier;
    [SerializeField] private TMP_InputField densityPercent;
    [SerializeField] private TMP_InputField vectorModifier;
    [SerializeField] private TMP_InputField helperLabel;
    
    /*
     * Attempts to create a damage and/or aging effect to the currently selected objects.
     */
    public void Execute(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        int width = 128, height = 128;
        foreach (var selected in currentlySelected)
        {  
            // 1. Find out if we are going to use any other objects as helpers (i.e. a window)
            Texture2D tex = CreateTextureMaskBase(selected, width, height);
            // 2. Once we have the texture to operate on, start by expanding it (signed distance field)
            tex = ApplySDFToTexture(tex);
            // 3. Apply random noise to it
            RandomNoise(tex);
            // 4. Texture mask that shit
        }
    }

    private void RandomNoise(Texture2D tex)
    {
        // First check what kind of noise we want (or if we want any at all)
        var noise = noiseModifier.options[noiseModifier.value].text;
        switch (noise)
        {
            case "perlin":
                Debug.Log("perlin");
                break;
            case "simplex":
                Debug.Log("simplex");
                break;
            case "none":
            default:
                Debug.Log("yo mama");
                break;
        }
        // Then apply that noise to the pixels with strength depending on the input alpha, I guess?
    }

    private Texture2D ApplySDFToTexture(Texture2D tex)
    {
        Texture2D dest = new Texture2D(tex.width, tex.height, TextureFormat.Alpha8, false);
        dest.alphaIsTransparency = true;
        SDFTextureGenerator.Generate(tex, dest, 0, 30, 30, RGBFillMode.Source);
        dest.Apply();
        byte[] bytes = dest.EncodeToPNG();
        var dirPath = Application.dataPath + "/SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image2" + ".png", bytes);
        AssetDatabase.Refresh();                                            // Force asset database to refresh
        return dest;
    }

    private Texture2D CreateTextureMaskBase(GameObject selectedObject, int width, int height)
    {
        // 1. Set up texture

        var helpLabelText = helperLabel.text;
        if (helpLabelText != "")
        {
            // Create a new background of black (representing nothing), with alpha channel also set to 0 (needed to fill SDF texture later)
            Texture2D backgroundTex = CreateSingleColorTexture2D(width, height, TextureFormat.Alpha8, false, new Color(0,0,0,0));
            // There is probably something to sample. Check if anything has the correct label
            List<GameObject> sampleSourceObjects = GetSampleSourceObjects(helpLabelText);
            var stepDist = selectedObject.GetComponent<Shape>().SizeExent / new Vector2(width, height);
            
            // Get the start position of where you should be stepping in world space (it makes sense)
            Vector2 currStep = new Vector2(selectedObject.transform.position.x - selectedObject.GetComponent<Shape>().SizeExent.x/2, selectedObject.transform.position.y - selectedObject.GetComponent<Shape>().SizeExent.y/2);
            
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    // Shoot out a ray from each point of the thing in the negative Z, towards camera. if we collide with one of the allowed objs, mark the pixel 
                    RaycastHit[] hits = Physics.RaycastAll(new Vector3(currStep.x, currStep.y, 1), -Vector3.forward, 100);
                    
                    foreach (var hit in hits)
                    {
                        if (sampleSourceObjects.Contains(hit.transform.gameObject))
                        {
                            // Set the color to white to begin a mask of where windows are
                            backgroundTex.SetPixel(x,y,Color.white);    
                        }
                    }
                    currStep.x += stepDist.x;
                }
                // start on next row
                currStep.x = selectedObject.transform.position.x - selectedObject.GetComponent<Shape>().SizeExent.x/2; 
                currStep.y += stepDist.y;
            }
            // Save texture to disk (maybe not necessary in the end, but really good for debugging purposes
            backgroundTex.Apply();
            byte[] bytes = backgroundTex.EncodeToPNG();
            var dirPath = Application.dataPath + "/SaveImages/";
            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "Image" + ".png", bytes);

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
        Debug.Log("how many rootchildren? " +rootChildren.Length);
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

    private Texture2D CreateSingleColorTexture2D(int width, int height, TextureFormat textureFormat, bool mipChain,
        Color color)
    {
        var texture = new Texture2D(width, height, textureFormat, mipChain);
        // Set colors alpha value to be 1 if black 0 if white?? yezzzzz
        Color[] pixels = Enumerable.Repeat(color, width * height).ToArray();
        Debug.Log("What is the default alpha value? " + pixels[0]);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;

    }
}