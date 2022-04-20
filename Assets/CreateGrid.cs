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
                newObjShapeComponent.GridRows.Add(currRow);
                Debug.DrawLine(new Vector3(x,y,0), new Vector3(x + extent.x , y ,0), Color.cyan, 10);
            }
            for (int j = 0; j < colsList.Length; j++)
            {
                float currCol = float.Parse(colsList[j]);
                float x = startPos.x + currCol;
                float y = startPos.y;
                newObjShapeComponent.GridCols.Add(currCol);
                Debug.DrawLine(new Vector3(x,y,0), new Vector3(x,y + extent.y,0), Color.yellow, 10);
            }
        }
    }

    private GameObject InitializeNewShape(Transform currentlySelectedsTransform)
    {
        GameObject newObj = new GameObject(label.text + " yo mama");
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
