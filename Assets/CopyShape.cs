using System.Collections;
using System.Collections.Generic;
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
        Debug.Log(label);
        List<GameObject> selectedObjs = GetSelectedObjects(label);
        foreach (var selection in selectedObjs)
        {
            Debug.Log("selection: " + selection);
            var newlyCreatedGO = Instantiate(selection, selection.transform.position, Quaternion.identity);
        }
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
