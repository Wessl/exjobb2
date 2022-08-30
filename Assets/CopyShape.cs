using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;

public class CopyShape : MonoBehaviour
{
    [SerializeField] private TMP_InputField labelInputField;
    [SerializeField] private TMP_InputField newLabelInputField;
    [SerializeField] private TMP_InputField posInputField;
    [SerializeField] private TMP_InputField rotInputField;
    [SerializeField] private TMP_InputField scaleInputField;
    
    public void Execute(SelectionHandler objectSelectionHandler)
    {
        // var currentSelection = objectSelectionHandler.currentSelection;
        string label = labelInputField.textComponent.text;
        Vector3 translateVec = GetVec3FromString(posInputField.textComponent.text);
        Vector3 rotateVec = GetVec3FromString(rotInputField.textComponent.text); 
        Vector3 scaleVec = GetVec3FromString(scaleInputField.textComponent.text); 
        Debug.Log(label);
        List<GameObject> selectedObjs = GetSelectedObjects(label);
        foreach (var selection in selectedObjs)
        {
            Debug.Log("selection: " + selection);
            var newlyCreatedGO = Instantiate(selection, selection.transform.position, Quaternion.identity);
            var newGOTransform = newlyCreatedGO.transform;
            newGOTransform.SetParent(selection.transform.parent);
            newGOTransform.Translate(translateVec);
            newGOTransform.Rotate(rotateVec);
            newGOTransform.localScale = new Vector3(newGOTransform.localScale.x * scaleVec.x,
                newGOTransform.localScale.y * scaleVec.y, newGOTransform.localScale.z * scaleVec.z);
        }
    }

    private Vector3 GetVec3FromString(string textComponentText)
    {
        if (!textComponentText.Contains(",")) throw new InvalidDataException("The Vector parameter is not properly separated by commas!");
        var splitText = textComponentText.Split(',');
        if (splitText.Length != 3) throw new InvalidDataException("Wrong dimension in vector, Should be 3.");
        Vector3 createdVec = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            var number = float.Parse(char.ToString(splitText[i][0]) , CultureInfo.InvariantCulture.NumberFormat);    // bloody hell mate. if you dont take the char and convert back to string it doesnt work. theres like a ghost or something hiding in it
            createdVec[i] = number;
        }
        return createdVec;
    }

    private List<GameObject> GetSelectedObjects(string helpLabelText)
    {
        // Start with the root
        GameObject root = GameObject.FindWithTag("Root");
        List<GameObject> foundObjects = new List<GameObject>();
        var rootChildren = root.GetComponentsInChildren<Shape>();
        Debug.Log("rootchildnre amount: " + rootChildren.Length);
        foreach (var rootChild in rootChildren)
        {
            var labels = rootChild.GetComponent<Shape>()?.Labels;
            if (labels is null) continue;
            foreach (var label in labels)
            {
                Debug.Log(label);
                var labelchararr = label.ToCharArray();
                var helplabeltextchararr = helpLabelText.ToCharArray();
                if (label.Equals(helpLabelText.Replace("\u200B", "")))  // zero width space showed up :))))))))))))))))
                {   
                    foundObjects.Add(rootChild.gameObject);
                }
                else
                {
                    Debug.Log("a damn, looks like " + label + " and " + helpLabelText + " are not the same!");
                    // for whatever reason facade and facade are not the same -_-
                }
            }
        }

        Debug.Log(foundObjects.Count);
        return foundObjects;
    }
}
