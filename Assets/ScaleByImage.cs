using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScaleByImage : MonoBehaviour
{
    [SerializeField] private TMP_InputField lowerBound;
    [SerializeField] private TMP_InputField upperBound;
    [SerializeField] private TMP_InputField selectFileInputField;
    [SerializeField] private TMP_InputField selectBackgroundShape;  // Typically this will be facade

    public void Execute()
    {
        var lower = float.Parse(lowerBound.text);
        var higher = float.Parse(upperBound.text);
        string path = selectFileInputField.text;
        
        Texture2D texture = new Texture2D(512, 512);

        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            texture.LoadImage(fileContent);
        }

        var backgroundShapeText = selectBackgroundShape.text;
        if (backgroundShapeText != "")
        {
            List<GameObject> sampledSourceObjects = GetSampleSourceObjects(backgroundShapeText); // This probably does not need to be a list?
            // Actually maybe completely rethink how we're doing this, this might be really dumb and I'm really tired
        }
        // now just get the positions of each selected object, and then get the same position on the input image
        // we also need a "background" in the same way as the damage and age
        // depending on how black or white it is, scale by a linearly interpolated value between lower and higher with rgb b/w value as t value
        // then literally just apply that as the scale... almost sounds too simple to be true! :D 

    }
    
    /*
     * This is an exact copy of the function in damage and age - maybe remake into static function? 
     */
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
}
