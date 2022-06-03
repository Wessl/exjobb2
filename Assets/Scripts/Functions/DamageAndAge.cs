using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatlikeCoding.SDFToolkit;
using TMPro;
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
        
        foreach (var selected in currentlySelected)
        {  
            // 1. Find out if we are going to use any other objects as helpers (i.e. a window)
            var tex = CreateTextureMaskBase(selected);
            // 2. Once we have the texture to operate on, start by expanding it (signed distance field)
            ApplySDFToTexture(tex);
            // 3. Apply random noise to it

            // 4. Texture mask that shit
        }
    }

    private void ApplySDFToTexture(Texture2D tex)
    {
        Texture2D dest = new Texture2D(100, 100);
        SDFTextureGenerator.Generate(tex, dest, 0, 40, 40, RGBFillMode.Black);
        byte[] bytes = dest.EncodeToPNG();
        var dirPath = Application.dataPath + "/SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image2" + ".png", bytes);
    }

    private Texture2D CreateTextureMaskBase(GameObject selectedObject)
    {
        // 1. Set up texture
        int height = 100, width = 100;
        
        var helpLabelText = helperLabel.text;
        if (helpLabelText != "")
        {
            Texture2D backgroundTex = CreateSingleColorTexture2D(width, height, TextureFormat.RGB24, false, Color.black);
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
            return CreateSingleColorTexture2D(width, height, TextureFormat.RGB24, false, Color.white);
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
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;

    }
}