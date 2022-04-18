using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action : MonoBehaviour
{
    public GameObject selectionParent;  // This is set by the Selex parent cell that creates it
    [SerializeField] private GameObject addShapePanel;
    [SerializeField] private GameObject createGridPanel;

    private GameObject lastSelection;   // used to hide the last selection easily
    private SelectionHandler _objectSelectionHandler;
    

    private void Start()
    {
        SelectAddShape();
    }

    public void FunctionCaller(int val)
    {
        // This is tremendously ugly, but who cares.
        switch (val)
        {
            case 0:
                SelectAddShape();
                break;
            case 1:
                SelectCreateGrid();
                break;
            default:
                lastSelection = null;
                break;
        }
    }

    private void SelectAddShape()
    {
        addShapePanel.SetActive(true);
        HideLastSelection();
        lastSelection = addShapePanel;
    }
    
    private void SelectCreateGrid()
    {
        createGridPanel.SetActive(true);
        HideLastSelection();
        lastSelection = createGridPanel;
    }

    public void ExecuteAddShape()
    {
        ObtainSelectionData();
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        addShapePanel.GetComponent<AddShape>().Execute();
    }

    public void ExecuteCreateGridLines()
    {
        createGridPanel.GetComponent<CreateGrid>().Execute();
    }

    private void HideLastSelection()
    {
        if (lastSelection != null)
        {
            lastSelection.SetActive(false);
        }
    }

    /*
     * Send down data from any/all selection panels in order to know what to operate on
     */
    public void ObtainSelectionData()
    {
        Debug.Log("trying to obtain selection data");   
        var selexObjParent = selectionParent.GetComponent<Selex>();
        // Now find all the things being selected by this...?
        var listOfGameObjects = new List<List<GameObject>>()
        {
            selexObjParent.AllAttributeSelections,
            selexObjParent.AllGroupsSelections,
            selexObjParent.AllTopologySelections
        };
        for (int i = 0; i < listOfGameObjects.Count; i++)
        {
            for (int y = 0; y < listOfGameObjects[i].Count; y++)
            {
                // What we have here is a single game object, containing some sort of selection data
                ExtractSelectionResultsDependingOnSelector(listOfGameObjects[i][y]);
            }
        }
        //while(selexObjParent != null){
        //selexObjParent = selexObjParent.GetComponent<Selex>().ParentCell;
        //}
    }
    
    // Tailor made extraction depending on 
    private void ExtractSelectionResultsDependingOnSelector(GameObject selector)
    {
        switch (selector.tag)
        {
            case "GroupSelectorGO":
                Debug.Log("group selector");
                break;
            case "TopologySelectorGO":
                Debug.Log("topology selector");
                TopologySelection(selector);
                break;
            case "AttributeSelectorGO":
                Debug.Log("attribute selector");
                AttributeSelection(selector);
                break;
        }
    }
    
    
    /*
     * Topology Selection
     */
    private void TopologySelection(GameObject topologyObject)
    {
        
        // This starts with a single shape as input... Wot. Just assume it starts wth the root for now...?
        var startpointObject = GameObject.FindWithTag("Root");
            
        var activeDropdownSelection = topologyObject.GetComponent<TMP_Dropdown>();
        if (!activeDropdownSelection) throw new InvalidOperationException("Topology object does not have a TMP Dropdown component attached!");
        string selectionName = activeDropdownSelection.options[activeDropdownSelection.value].text;
        
        var startpointShape = startpointObject.GetComponent<Shape>();
        switch (selectionName)
        {
            case "child()":
                var children = startpointShape.children;
                _objectSelectionHandler.currentSelection = children;
                break;
            case "parent()":
                var parent = startpointShape.parent;
                _objectSelectionHandler.currentSelection = new List<GameObject>(){ parent };
                break;
            case "descendant()":
                Debug.Log("descendant");
                List<GameObject> descendants = RecursivelyGetDescendants(startpointShape);
                break;
            case "root()":
                var root = startpointShape.gameObject;
                _objectSelectionHandler.currentSelection = new List<GameObject>(){ root };
                break;
            case "neighbour()":
                var neighbours = startpointShape.neighbours;
                _objectSelectionHandler.currentSelection = neighbours;
                break;
            case "contained()":
                // This will essentially have to check the borders of every gameObject under root
                // And make sure they are within the input's size/borders
                break;
            
        }
    }

    private List<GameObject> RecursivelyGetDescendants(Shape shape)
    {
        var children = shape.children;
        var allObjs = new List<GameObject> {shape.gameObject};
        foreach (var child in children)
        {
            Debug.Log("descendant: " + child);
            allObjs.AddRange(RecursivelyGetDescendants(child.GetComponent<Shape>()));
        }

        return allObjs;
    }
    
    /*
     * Attribute Selection
     */

    private void AttributeSelection(GameObject selector)
    {
        // name based component getting - bad
        var attrSelection = selector.GetComponent<AttrSelection>();
        var attributeNameDropdown = attrSelection.AttributeNameDropdown.GetComponent<TMP_Dropdown>();
        var operatorDropdown = attrSelection.OperatorDropdown.GetComponent<TMP_Dropdown>();
        var valueField = attrSelection.ValueInputfield.GetComponent<TMP_InputField>();

        Debug.Log(attributeNameDropdown.value);
        Debug.Log(operatorDropdown.value);
        Debug.Log(valueField.text);
        
    }
}
