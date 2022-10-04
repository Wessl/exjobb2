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
        // why so many replacements? zero width space. :/
        string label = labelInputField.textComponent.text.Replace("\u200B", "");
        string newlabel = newLabelInputField.textComponent.text.Replace("\u200B", "");
        var newlabelchararr = newlabel.ToCharArray();
        Vector3 translateVec = GetVec3FromString(posInputField.textComponent.text.Replace("\u200B", ""), new Vector3(0,0,0));
        Vector3 rotateVec = GetVec3FromString(rotInputField.textComponent.text.Replace("\u200B", ""), new Vector3(0,0,0)); 
        Vector3 scaleVec = GetVec3FromString(scaleInputField.textComponent.text.Replace("\u200B", ""), new Vector3(1,1,1)); 
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
            newlyCreatedGO.GetComponent<Shape>().Labels = new List<string>();
            newGOTransform.GetComponent<Shape>().Labels.Add(newlabel);
            // Add as child to parent's list
            newGOTransform.parent.GetComponent<Shape>().children.Add(newlyCreatedGO);
        }
    }

    private Vector3 GetVec3FromString(string textComponentText, Vector3 defaultVec)
    {
        if (textComponentText.Length == 0) return defaultVec;
            if (!textComponentText.Contains(",")) throw new InvalidDataException("The Vector parameter is not properly separated by commas!");
        var splitText = textComponentText.Split(',');
        if (splitText.Length != 3) throw new InvalidDataException("Wrong dimension in vector, Should be 3.");
        Vector3 createdVec = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            var number = float.Parse(splitText[i], CultureInfo.InvariantCulture.NumberFormat);
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
                if (label.Equals(helpLabelText))
                {   
                    foundObjects.Add(rootChild.gameObject);
                }
            }
        }

        Debug.Log(foundObjects.Count);
        return foundObjects;
    }
}
