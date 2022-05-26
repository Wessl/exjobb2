using System.Collections;
using System.Collections.Generic;
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
        int height = 1000, width = 1000;
        Texture2D backgroundTex = new Texture2D(width, height);
        var helpLabelText = helperLabel.text;
        if (helpLabelText != "")
        {
            // There is probably something to sample. Check if anything has the correct label
            List<GameObject> sampleSourceObjects = GetSampleSourceObjects(helpLabelText);
            var stepDist = selectedObject.GetComponent<Shape>().SizeExent / new Vector2(width, height);
            Vector2 currStep = new Vector2();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Sample here
                    RaycastHit[] hits = Physics.RaycastAll(new Vector3(currStep.x, currStep.y, 0), Vector3.forward, 100);
                    Debug.Log("hits: " + hits.Length);
                    currStep.x += stepDist.x;
                }

                currStep.y += stepDist.y;
            }

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
        var rootChildren = root.GetComponentsInChildren<Transform>();
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
}