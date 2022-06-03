using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            GetHelpObjectsInfo(selected);
            // 2. Once we have the texture to operate on, start by expanding it (forget the name of it)
            
            // 3. Apply random noise to it
            
            // 4. Texture mask that shit
        }
    }

    private void GetHelpObjectsInfo(GameObject selectedObject)
    {
        // 1. Set up texture
        int height = 100, width = 100;
        Texture2D backgroundTex = CreateSingleColorTexture2D(width, height, TextureFormat.RGB24, false, Color.black);
        var helpLabelText = helperLabel.text;
        if (helpLabelText != "")
        {
            // There is probably something to sample. Check if anything has the correct label
            List<GameObject> sampleSourceObjects = GetSampleSourceObjects(helpLabelText);
            var stepDist = selectedObject.GetComponent<Shape>().SizeExent / new Vector2(width, height);
            // Get the start position of where you should be stepping in world space (it makes sense)
            Vector2 currStep = new Vector2(selectedObject.transform.position.x - selectedObject.GetComponent<Shape>().SizeExent.x/2, selectedObject.transform.position.y - selectedObject.GetComponent<Shape>().SizeExent.y/2);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Sample here
                    RaycastHit[] hits = Physics.RaycastAll(new Vector3(currStep.x, currStep.y, 1), -Vector3.forward, 100);
                    
                    foreach (var hit in hits)
                    {
                        if (sampleSourceObjects.Contains(hit.transform.gameObject))
                        {
                            backgroundTex.SetPixel(x,y,Color.white);
                        }
                    }
                    
                    currStep.x += stepDist.x;
                }

                currStep.x = selectedObject.transform.position.x - selectedObject.GetComponent<Shape>().SizeExent.x/2; // set back to 0, start on next row
                currStep.y += stepDist.y;
            }
            // Save texture to disk (maybe not necessary in the end, but really good for debugging purposes
            byte[] bytes = backgroundTex.EncodeToPNG();
            var dirPath = Application.dataPath + "/../SaveImages/";
            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
            
            // Make sure every object you are comparing against has a collider

            // Shoot out a ray from each point of the thing. if we collide with one of the allowed objs, mark the pixel 
        }
        else
        {
            // Idk do nothing bitch
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
                        Debug.Log("OH SHIT YOU FOUND IT BITCH. ");
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
        Color[] pixels = Enumerable.Repeat(color, width * height).ToArray();
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;

    }
}