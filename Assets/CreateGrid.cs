using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    [SerializeField] private TMP_InputField label;
    [SerializeField] private TMP_InputField rows;
    [SerializeField] private TMP_InputField columns;

    

    private void Start()
    {
       
    }

    public void Execute(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        List<GameObject> newlyCreatedObjects = new List<GameObject>();
        var rowsList = rows.text.Split(',');
        var colsList = columns.text.Split(',');
        
        // Operate on the existing input shape from here...
        for (int i = 0; i < currentlySelected.Count; i++)
        {
            var newObj = InitializeNewShape(currentlySelected[i].transform);
            var startPos = currentlySelected[i].transform.position;
            var newObjShapeComponent = newObj.GetComponent<Shape>();
            var extent = newObjShapeComponent.SizeExent;
            Debug.Log("extent: " + extent);
            for (int j = 0; j < rowsList.Length; j++)
            {
                float currRow = float.Parse(rowsList[j]);
                float x = startPos.x;
                float y = startPos.y + currRow;
                if (y < (startPos.y + extent.y))
                {
                    newObjShapeComponent.GridRows.Add(currRow);
                    Debug.DrawLine(new Vector3(x,y,0), new Vector3(x + extent.x , y ,0), Color.cyan, 100);
                }
                
            }
            for (int j = 0; j < colsList.Length; j++)
            {
                float currCol = float.Parse(colsList[j]);
                float x = startPos.x + currCol;
                float y = startPos.y;
                if (x < (startPos.x + extent.x))
                {
                    newObjShapeComponent.GridCols.Add(currCol);
                    Debug.DrawLine(new Vector3(x,y,0), new Vector3(x,y + extent.y,0), Color.yellow, 100);
                }
            }
            // Now that we have all the knowledge of where grids are, let's actually create the grid components
            var cols = newObjShapeComponent.GridCols;
            var rows = newObjShapeComponent.GridRows;
            cols.Insert(0, 0);
            cols.Insert(cols.Count, extent.x);
            rows.Insert(0, 0);
            rows.Insert(rows.Count, extent.y);
            for (int j = 0; j < cols.Count-1; j++)
            {
                for (int y = 0; y < rows.Count-1; y++)
                {
                    GameObject gridPart = new GameObject("gridPart");
                    gridPart.name = label.text;
                    gridPart.AddComponent<Shape>();
                    var gridPartShapeComponent = gridPart.GetComponent<Shape>();
                    gridPart.transform.position = new Vector3(cols[j] + startPos.x, rows[y] + startPos.y, 0);     // Sets the start pos for this grid component
                    gridPartShapeComponent.Start();
                    gridPart.transform.parent = currentlySelected[i].transform;
                    gridPartShapeComponent.parent = currentlySelected[i];
                    // Now find the size extent 
                    var sizeExtentX = Mathf.Abs(cols[j] - cols[j + 1]);
                    var sizeExtentY = Mathf.Abs(rows[y] - rows[y + 1]);
                    gridPartShapeComponent.SetupSizeExtent(new Vector2(sizeExtentX, sizeExtentY));
                    
                    // Finally, add each one to the list of currently selected?
                    newlyCreatedObjects.Add(gridPart);
                }
            }
        }
        currentlySelected.AddRange(newlyCreatedObjects);
    }

    private GameObject InitializeNewShape(Transform currentlySelectedsTransform)
    {
        GameObject newObj = new GameObject(label.text);
        var startTransform = currentlySelectedsTransform;
        newObj.AddComponent<Shape>();
        var newObjShapeComponent = newObj.GetComponent<Shape>();
        newObjShapeComponent.Start();                               //Initialize the component bro
        newObjShapeComponent.parent = currentlySelectedsTransform.gameObject;
        newObjShapeComponent.SetupSizeExtent();
        newObj.transform.parent = startTransform;
        newObj.name = label.text;
        
        return newObj;
    }
}
