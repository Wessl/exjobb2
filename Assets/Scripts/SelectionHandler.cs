using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        var rootChildren = root.GetComponentsInChildren<Shape>();
        rootChildren = rootChildren.Skip(1).ToArray();  // don't include root // update: actually you wanna include root otherwise if you only start with root there is nothing to start building on
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
    
    /*
     * Grouping functionality
     */
    public void GroupCols()
    {
        List<GameObject> createdCols = new List<GameObject>();
        for (int i = 0; i < currentSelection.Count; i++)
        {

            List<GameObject> virtualObjectsToGroup = new List<GameObject>();
            var selection = currentSelection[i];
            var startPoint = selection;

            virtualObjectsToGroup.Add(selection);
            // Go upwards and find everything above
            while (GetVirtualObjectAbove(selection) is var above && above != null)
            {
                selection = above;
                virtualObjectsToGroup.Add(selection);
            }
            // Now start from the first one and go downwards 
            selection = startPoint;
            while (GetVirtualObjectBelow(selection) is var below && below != null)
            {
                selection = below;
                virtualObjectsToGroup.Add(selection);
            }
            // Now we have all the parts necessary to turn all the components into a single large column
            // However, if the selection did not operate on a row or column, i.e. was solo shape, ignore it
            if (virtualObjectsToGroup.Count > 1)
            {
                var newShape = CombineShape(virtualObjectsToGroup, selection);
                createdCols.Add(newShape);
            }
            // Assigning neighbours?
            CreateGrid.AssignShapeRelations(createdCols);
        }
    }

    public void GroupRows()
    {
        List<GameObject> createdRows = new List<GameObject>();
        for (int i = 0; i < currentSelection.Count; i++)
        {
            List<GameObject> virtualObjectsToGroup = new List<GameObject>();
            var selection = currentSelection[i];
            var startPoint = selection;
            virtualObjectsToGroup.Add(selection);
            // Go to the right
            while (GetVirtualObjectRight(selection) is var right && right != null)
            {
                selection = right;
                virtualObjectsToGroup.Add(selection);
                
            }
            // Now start from the original one and go to the left
            selection = startPoint;
            while (GetVirtualObjectLeft(selection) is var left && left != null)
            {
                selection = left;
                virtualObjectsToGroup.Add(selection);
            }
            // Now we have all the parts necessary to turn all the components into a single large column
            // However, if the selection did not operate on a row or column, i.e. was solo shape, ignore it
            if (virtualObjectsToGroup.Count > 1)
            {
                var newShape = CombineShape(virtualObjectsToGroup, selection);
                createdRows.Add(newShape);
            }
            // Assigning neighbours
            CreateGrid.AssignShapeRelations(createdRows);
        }
    }

    public void GroupRegions()
    {
        
    }

    /*  Operating on e.g. a row or column of selected shapes...
     *  1. Go through every collected virtual construction shape
        2. save the largest extent in positive and negative x,y directions
        3. delete all current virtual objects, remove from currentSelection
        4. create new virtual object with the new extent instead
     */
    private GameObject CombineShape(List<GameObject> virtualObjectsToGroup, GameObject selection)
    {
      
        List<string> labels = new List<string>();
        Vector2 largest = new Vector2(float.MinValue, float.MinValue);
        Vector2 smallest = new Vector2(float.MaxValue, float.MaxValue);
        var startPos = virtualObjectsToGroup.OrderBy(o => Vector3.Distance(o.transform.position, Vector3.zero)).ToList()[0].transform.position;
        
        for (int y = 0; y < virtualObjectsToGroup.Count; y++)
        {
            var vObjToGroup = virtualObjectsToGroup[y];
            if (vObjToGroup.CompareTag("Root")) continue;                           // this is bad practice, but I can't really think of another way
            var currShape = vObjToGroup.GetComponent<Shape>();
            var extent = currShape.SizeExent;
            var currX = extent.x + vObjToGroup.transform.position.x;
            var currY = extent.y + vObjToGroup.transform.position.y;
            if (currX > largest.x) largest.x = currX;
            if (currY > largest.y) largest.y = currY;
            if (currX-extent.x < smallest.x) smallest.x = currX-extent.x;
            if (currY-extent.y < smallest.y) smallest.y = currY-extent.y;
            labels.AddRange(selection.GetComponent<Shape>().Labels);
            currentSelection.Remove(vObjToGroup);
            DestroyShapeSafely(vObjToGroup.GetComponent<Shape>());
        }
        // Create a new virtual object which has these properties
        // We also probalby need to also assign new shape relations and shit... fuck
        GameObject groupedCol = new GameObject("gridPart");

        groupedCol.AddComponent<Shape>();
        var groupedColShape = groupedCol.GetComponent<Shape>();
        groupedCol.transform.position = new Vector3(smallest.x, smallest.y, 0);     // Sets the start pos for this grid component
        groupedColShape.Start();
        
        // Parent assignment
        if (selection.transform.parent != null)
        {
            groupedCol.transform.parent = selection.transform.parent; 
            groupedColShape.parent = selection.transform.parent.gameObject;
        }

        // Handle labeling
        if (labels.Count > 0) groupedColShape.Labels.AddRange(labels);

        // Set the Shape Type
        groupedColShape.currentType = Shape.ShapeType.Virtual;
                
        // Assign the already known size extent
        var sizeExtentX = largest.x - startPos.x;
        var sizeExtentY = largest.y - startPos.y;
        groupedColShape.SetupSizeExtent(new Vector2(sizeExtentX, sizeExtentY));
                
        // Finally, add each one to the list of currently selected?
        currentSelection.Add(groupedCol);
        
        // Debug
        Debug.DrawLine(new Vector3(smallest.x, smallest.y, 0), new Vector3(smallest.x, largest.y, 0), Color.red, 50);
        Debug.DrawLine(new Vector3(smallest.x, smallest.y, 0), new Vector3(largest.x, smallest.y, 0), Color.red, 50);
        Debug.DrawLine(new Vector3(largest.x, smallest.y, 0), new Vector3(largest.x, largest.y, 0), Color.red, 50);
        Debug.DrawLine(new Vector3(smallest.x, largest.y, 0), new Vector3(largest.x, largest.y, 0), Color.red,50);

        return groupedCol;
    }

    private GameObject GetVirtualObjectAbove(GameObject currentObj)
    {
        // Do the same thing for below, left, right, then make it recursive so we get everything in a column or row
        var currentShape = currentObj.GetComponent<Shape>();
        var neighbours = currentShape.neighbours;
        var tolerance = 0.001f;
        foreach (var neighbour in neighbours)
        {
            if (Math.Abs(currentShape.transform.position.y + currentShape.SizeExent.y - neighbour.transform.position.y) <
                tolerance )
            {
                return neighbour;
            }
        }

        return null;
    }

    private GameObject GetVirtualObjectBelow(GameObject currentObj)
    {
        var currentShape = currentObj.GetComponent<Shape>();
        var neighbours = currentShape.neighbours;
        var tolerance = 0.001f;
        foreach (var neighbour in neighbours)
        {
            if (Math.Abs(currentShape.transform.position.y - currentShape.SizeExent.y - neighbour.transform.position.y) <
                tolerance )
            {
                return neighbour;
            }
        }

        return null;
    }

    private GameObject GetVirtualObjectRight(GameObject currentObj)
    {
        var currentShape = currentObj.GetComponent<Shape>();
        var neighbours = currentShape.neighbours;
        var tolerance = 0.001f;
        foreach (var neighbour in neighbours)
        {
            if (Math.Abs(currentShape.transform.position.x + currentShape.SizeExent.x - neighbour.transform.position.x) <
                tolerance )
            {
                return neighbour;
            }
        }

        return null;
    }

    private GameObject GetVirtualObjectLeft(GameObject currentObj)
    {
        var currentShape = currentObj.GetComponent<Shape>();
        var neighbours = currentShape.neighbours;
        var tolerance = 0.001f;
        foreach (var neighbour in neighbours)
        {
            if (Math.Abs(currentShape.transform.position.x - currentShape.SizeExent.x - neighbour.transform.position.x) <
                tolerance )
            {
                return neighbour;
            }
        }

        return null;
    }

    public void DestroyShapeSafely(Shape shape)
    {
        GameObject obj = shape.gameObject;
        shape.parent.GetComponent<Shape>().children.Remove(obj);
        foreach (var neighbour in shape.neighbours)
        {
            neighbour.GetComponent<Shape>().neighbours.RemoveAll(s => s == obj);
        }
        Destroy(obj);
    }
    
}
