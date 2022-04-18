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

    private float[,] grid;

    public float[,] Grid => grid;

    private GameObject blankObject;

    private void Start()
    {
       
    }

    public void Execute(SelectionHandler objectSelectionHandler)
    {
        blankObject = new GameObject(label.text);
        var currentlySelected = objectSelectionHandler.currentSelection;
        var rowsList = rows.text.Split(',');
        var colsList = columns.text.Split(',');

        grid = new float[rowsList.Length, colsList.Length];
        
        // Operate on the existing input shape from here...
        for (int i = 0; i < currentlySelected.Count; i++)
        {
            // instantiate?
            var startLocation = currentlySelected[i].transform;
            var newObj = Instantiate(blankObject, startLocation.position, Quaternion.identity);
            newObj.name = label.text;
            for (int y = 0; y < rowsList.Length; y++)
            {
                float currRow = float.Parse(rowsList[y]);
                Debug.DrawLine(new Vector3(0,currRow,0), new Vector3(10,currRow,0), Color.cyan, 10);
            }
            for (int y = 0; y < colsList.Length; y++)
            {
                float currCol = float.Parse(colsList[y]);
                Debug.DrawLine(new Vector3(currCol,0,0), new Vector3(currCol,10,0), Color.yellow, 10);
            }
        }
    }
}
