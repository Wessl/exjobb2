using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CreateGrid : MonoBehaviour
{
    [SerializeField] private TMP_InputField label;
    [SerializeField] private TMP_InputField rows;
    [SerializeField] private TMP_InputField columns;

    [SerializeField] private GameObject rowContainer;
    private List<GameObject> rowsGOList;
    [SerializeField] private GameObject colContainer;
    private List<GameObject> colsGOList;

    private List<Tuple<float,string>> combinedRowsList;
    private List<Tuple<float,string>> combinedColsList;

    [SerializeField] private int moveDownAmnt;

    private List<Tuple<int, string>> labels = new List<Tuple<int, string>>();

    private void Start()
    {
        rowsGOList = new List<GameObject>();
        colsGOList = new List<GameObject>();
        combinedRowsList = new List<Tuple<float, string>>();
        combinedColsList = new List<Tuple<float, string>>();
        rowsGOList.Add(rowContainer);
        colsGOList.Add(colContainer);
    }

    public void AnotherGridRow()
    {
        var newRowContainer = Instantiate(rowContainer, rowsGOList[rowsGOList.Count-1].transform.position + Vector3.down * moveDownAmnt, Quaternion.identity);
        newRowContainer.transform.SetParent(this.transform);
        newRowContainer.transform.SetSiblingIndex(rowsGOList[rowsGOList.Count-1].transform.GetSiblingIndex()+1);
        rowsGOList.Add(newRowContainer);
        // Expand
        this.transform.parent.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -moveDownAmnt);
        this.transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, moveDownAmnt);

    }

    public void AnotherGridCol()
    {
        var newColContainer = Instantiate(colContainer, colsGOList[colsGOList.Count-1].transform.position + Vector3.down * moveDownAmnt, Quaternion.identity);
        newColContainer.transform.SetParent(this.transform);
        newColContainer.transform.SetSiblingIndex(colsGOList[colsGOList.Count-1].transform.GetSiblingIndex()+1);

        colsGOList.Add(newColContainer);
        // Expand
        this.transform.parent.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -moveDownAmnt);
        this.transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, moveDownAmnt);
        // And because this is anchored to the bottom, move everything up one step as well
        foreach (var col in colsGOList)
        {
            col.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, moveDownAmnt);
        }
    }

    public void Execute(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        List<GameObject> newlyCreatedObjects = new List<GameObject>();
        
        CombineRowColListsIntoOne();    // Will populate rowsList and colsList

        // Operate on the existing input shape from here...
        for (int i = 0; i < currentlySelected.Count; i++)
        {
            var newObj = InitializeNewShape(currentlySelected[i].transform);
            var startPos = currentlySelected[i].transform.position;
            var newObjShapeComponent = newObj.GetComponent<Shape>();
            var extent = newObjShapeComponent.SizeExent;
            Debug.Log("extent: " + extent);
            for (int j = 0; j < combinedRowsList.Count; j++)
            {
                float currRow = combinedRowsList[j].Item1;
                float x = startPos.x;
                float y = startPos.y + currRow;
                if (y < (startPos.y + extent.y))
                {
                    newObjShapeComponent.GridRows.Add(currRow);
                    Debug.DrawLine(new Vector3(x,y,0), new Vector3(x + extent.x , y ,0), Color.cyan, 100);
                }
                
            }
            for (int j = 0; j < combinedColsList.Count; j++)
            {
                float currCol = combinedColsList[j].Item1;
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
            // Add in start and end points for the grid (assuming creator only makes inbetween bits)
            //cols.Insert(0, 0);
            //cols.Insert(cols.Count, extent.x);
            //rows.Insert(0, 0);
            //rows.Insert(rows.Count, extent.y);
            for (int j = 0; j < cols.Count-1; j++)
            {
                // column name?
                var colLabel = combinedColsList[j].Item2;
                for (int y = 0; y < rows.Count-1; y++)
                {
                    var rowLabel = combinedRowsList[y].Item2;
                    GameObject gridPart = new GameObject("gridPart");

                    gridPart.AddComponent<Shape>();
                    var gridPartShapeComponent = gridPart.GetComponent<Shape>();
                    gridPart.transform.position = new Vector3(cols[j] + startPos.x, rows[y] + startPos.y, 0);     // Sets the start pos for this grid component
                    gridPartShapeComponent.Start();
                    gridPart.transform.parent = currentlySelected[i].transform;
                    gridPartShapeComponent.parent = currentlySelected[i];
                    
                    // Handle labeling
                    gridPartShapeComponent.Labels.Add(rowLabel);
                    gridPartShapeComponent.Labels.Add(colLabel);
                    
                    // Set the Shape Type
                    gridPartShapeComponent.currentType = Shape.ShapeType.Virtual;
                    
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

    // Both combines every row and col list into one, but also saves the labels
    private void CombineRowColListsIntoOne()
    {
        foreach (var colObj in colsGOList)
        {
            var inputFields = colObj.GetComponentsInChildren<TMP_InputField>();
            // There are two inputfield children - one has a comma separated list of values and is tagged, the other is not tagged and contains the label
            for( int i = 0; i < inputFields.Length; i++)
            {
                var inputField = inputFields[i];
                if (inputField.CompareTag("InputField"))
                {
                    var cols = inputField.text.Split(',');
                    string otherInputField = inputFields[(i + 1) % (inputFields.Length)].text;
                    foreach (var column in cols)
                    {
                        var colValLabel = new Tuple<float, string>(float.Parse(column), otherInputField);
                        combinedColsList.Add(colValLabel);
                    }
                }
            }
        }
        foreach (var rowObj in rowsGOList)
        {
            var inputFields = rowObj.GetComponentsInChildren<TMP_InputField>();
            // There are two inputfield children
            for (int i = 0; i < inputFields.Length; i++)
            {
                var inputField = inputFields[i];
                if (inputField.CompareTag("InputField"))
                {
                    var rows = inputField.text.Split(',');
                    string otherInputField = inputFields[(i + 1) % (inputFields.Length)].text;
                    foreach (var row in rows)
                    {
                        var rowValLabel = new Tuple<float, string>(float.Parse(row), otherInputField);
                        combinedRowsList.Add(rowValLabel);
                    }
                }
            }
        }
    }

    private GameObject InitializeNewShape(Transform currentlySelectedsTransform)
    {
        GameObject newObj = new GameObject();
        var startTransform = currentlySelectedsTransform;
        newObj.AddComponent<Shape>();
        var newObjShapeComponent = newObj.GetComponent<Shape>();
        newObjShapeComponent.Start();                               //Initialize the component bro
        newObjShapeComponent.parent = currentlySelectedsTransform.gameObject;
        newObjShapeComponent.SetupSizeExtent();
        newObj.transform.parent = startTransform;

        return newObj;
    }
}
