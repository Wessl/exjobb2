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

    public void Execute(SelectionHandler selectionHandler)
    {
        var lower = float.Parse(lowerBound.text);
        var higher = float.Parse(upperBound.text);
        string path = "Assets/" + selectFileInputField.text;
        
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
            GameObject firstFoundSourceObj = sampledSourceObjects[0];
            Shape backgroundShape = firstFoundSourceObj.GetComponent<Shape>();
            Vector2 sizeExtent = backgroundShape.SizeExent;
            Vector2 scaleFactor = new Vector2(texture.width / sizeExtent.x, texture.height / sizeExtent.y);
            foreach (var selection in selectionHandler.currentSelection)
            {
                var pos = selection.transform.position;
                var pixelValAtPos = texture.GetPixel(Mathf.FloorToInt(pos.x * scaleFactor.x), Mathf.FloorToInt(pos.y * scaleFactor.y));
                var t = pixelValAtPos.a;    // r or a is different depending on whether or not single channel rgb or alpha
                var scaleVal = Mathf.Lerp(lower, higher, t);
                selection.transform.localScale *= scaleVal;
            }
        }
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
        foreach (var rootChild in rootChildren)
        {
            var labels = rootChild.GetComponent<Shape>()?.Labels;
            if (labels is null)
            {
                // do nothing
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
