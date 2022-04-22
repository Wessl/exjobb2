using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{
    public List<GameObject> currentSelection;

    [SerializeField] private GameObject root;
    // Start is called before the first frame update
    void Start()
    {
        currentSelection = new List<GameObject>();
        var rootChildren = root.GetComponentsInChildren<Transform>();
        foreach (var rootChild in rootChildren)
        {
            currentSelection.Add(rootChild.gameObject);
        }
    }
    
    // ATTRIBUTE SELECTION FUNCTIONS
    public void Attr_IsOdd()
    {
        List<GameObject> newSelection = new List<GameObject>();
        for (int i = 0; i < currentSelection.Count; i++)
        {
            if (i % 2 != 0)
            {
                newSelection.Add(currentSelection[i]);
            }
        }

        currentSelection = newSelection;
    }

    public void Attr_IsEven()
    {
        List<GameObject> newSelection = new List<GameObject>();
        for (int i = 0; i < currentSelection.Count; i++)
        {
            if (i % 2 == 0)
            {
                newSelection.Add(currentSelection[i]);
            }
        }

        currentSelection = newSelection;
    }

    public void Attr_IsEmpty()
    {
        List<GameObject> newSelection = new List<GameObject>();
        foreach (var selection in currentSelection)
        {
            // Only include those that have no children? 
            var children = selection.GetComponent<Shape>().children;
            if (children == null || children.Count == 0)
            {
                newSelection.Add(selection);
            }
        }

        currentSelection = newSelection;
    }

    public void Attr_Label(string labelOperator, string labelValue)
    {
        List<GameObject> newSelection = new List<GameObject>();
        Debug.Log("labeloperator: " +labelOperator);
        // I guess only handle == and != ?
        if (labelOperator != "==" && labelOperator != "!=")
        {
            throw new InvalidOperationException("Label comparison cannot be done with selected operator! Only '==' or '!=' are allowed.");
        }

        if (labelOperator.Equals("=="))
        {
            // Label is referred to as a list of strings. yeah
            foreach (var selection in currentSelection)
            {
                var labels = selection.GetComponent<Shape>().Labels;
                foreach (var label in labels)
                {
                    if (label.Equals(labelValue))
                    {
                        newSelection.Add(selection);
                    }
                }
            }
        }
        else if (labelOperator.Equals("!="))
        {
            foreach (var selection in currentSelection)
            {
                if (! selection.name.Equals(labelValue))
                {
                    newSelection.Add(selection);
                }
            }
        }

        foreach (var selection in newSelection)
        {
            Debug.Log("selection: " + selection.name);
        }
        currentSelection = newSelection;
    }

    public void Attr_Idx(string labelOperator, string valueField)
    {
        // TODO: Implement "in"
        int index = Int32.Parse(valueField);
        List<GameObject> newSelection = new List<GameObject>();
        switch (labelOperator)
        {
            case "==":
                newSelection.Add(currentSelection[index]);
                break;
            case "!=":
                newSelection.AddRange(currentSelection);
                newSelection.RemoveAt(index);
                break;
            case ">":
                for (int i = index + 1; i < currentSelection.Count; i++)
                {
                    newSelection.Add(currentSelection[i]);
                }
                break;
            case "<":
                for (int i = 0; i < index; i++)
                {
                    newSelection.Add(currentSelection[i]);
                }
                break;
            case ">=":
                for (int i = index; i < currentSelection.Count; i++)
                {
                    newSelection.Add(currentSelection[i]);
                }
                break;
            case "<=":
                for (int i = 0; i < index + 1; i++)
                {
                    newSelection.Add(currentSelection[i]);
                }
                break;
            case "in":
                throw new NotImplementedException("'In' has not been implemented yet!");

            default:
                Debug.Log("Nothing was properly selected");
                break;
        }
        
        
        
        
        currentSelection = newSelection;
    }

    public void Attr_Pattern(GameObject patternPanel)
    {
        var inputField = patternPanel.GetComponentInChildren<InputField>();
        var regexString = inputField.text;
        throw new NotImplementedException();
    }

    public void Attr_ColIdx()
    {
        throw new NotImplementedException();
    }

    public void Attr_RowIdx()
    {
        throw new NotImplementedException();
    }

    
}
