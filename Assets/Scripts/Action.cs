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
    [SerializeField] private GameObject rotatePanel;
    [SerializeField] private GameObject scalePanel;
    [SerializeField] private GameObject translatePanel;
    [SerializeField] private GameObject damageAndAgePanel;
    [SerializeField] private GameObject scaleByImagePanel;
    [SerializeField] private GameObject copyShapePanel;
    

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
                SelectCopyShape();
                break;
            case 2:
                SelectGroup();
                break;
            case 3:
                SelectCreateGrid();
                break;
            case 4:
                SelectSetAttrib();
                break;
            case 5:
                SelectRotate();
                break;
            case 6:
                SelectScale();
                break;
            case 7:
                SelectTranslate();
                break;
            case 8:
                SelectDamageAndAge();
                break;
            case 9:
                SelectScaleByImage();
                break;
            default:
                lastSelection = null;
                break;
        }
    }

    private void SelectGroup()
    {
        throw new NotImplementedException();
    }

    private void SelectSetAttrib()
    {
        throw new NotImplementedException();
    }

    private void SelectTransform()
    {
        throw new NotImplementedException();
    }
    private void SelectCopyShape()
    {
        copyShapePanel.SetActive(true);
        HideLastSelection();
        lastSelection = copyShapePanel;
    }
    private void SelectDamageAndAge()
    {
        damageAndAgePanel.SetActive(true);
        HideLastSelection();
        lastSelection = damageAndAgePanel;
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
    
    private void SelectRotate()
    {
        // First set up the UI shit then just add the functionality... easy? yes. useful? maybe, maybe not
        rotatePanel.SetActive(true);
        HideLastSelection();
        lastSelection = rotatePanel;
    }
    
    private void SelectScale()
    {
        // First set up the UI shit then just add the functionality... easy? yes. useful? maybe, maybe not
        scalePanel.SetActive(true);
        HideLastSelection();
        lastSelection = scalePanel;
    }
    
    private void SelectTranslate()
    {
        // First set up the UI shit then just add the functionality... easy? yes. useful? maybe, maybe not
        translatePanel.SetActive(true);
        HideLastSelection();
        lastSelection = translatePanel;
    }
    
    private void SelectScaleByImage()
    {
        // First set up the UI shit then just add the functionality... easy? yes. useful? maybe, maybe not
        scaleByImagePanel.SetActive(true);
        HideLastSelection();
        lastSelection = scaleByImagePanel;
    }

    public void ExecuteCopyShape()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        copyShapePanel.GetComponent<CopyShape>().Execute(_objectSelectionHandler);
    }

    public void ExecuteAddShape()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        addShapePanel.GetComponent<AddShape>().Execute(_objectSelectionHandler);
    }

    public void ExecuteCreateGridLines()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        createGridPanel.GetComponent<CreateGrid>().Execute(_objectSelectionHandler);
    }

    public void ExecuteRotate()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        rotatePanel.GetComponent<Transforms>().ExecuteRotate(_objectSelectionHandler);
    }
    
    public void ExecuteScale()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        scalePanel.GetComponent<Transforms>().ExecuteScale(_objectSelectionHandler);
    }
    
    public void ExecuteTranslation()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        translatePanel.GetComponent<Transforms>().ExecuteTranslation(_objectSelectionHandler);
    }

    public void ExecuteDamageAndAge()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        damageAndAgePanel.GetComponent<DamageAndAge>().Execute(_objectSelectionHandler);
    }

    public void ExecuteScaleByImage()
    {
        _objectSelectionHandler = GameObject.FindWithTag("Root").GetComponent<SelectionHandler>();
        ObtainSelectionData();
        scaleByImagePanel.GetComponent<ScaleByImage>().Execute(_objectSelectionHandler);
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
        // start with selecting everything
        // _objectSelectionHandler.SelectEverything();
        var selexObj = selectionParent.GetComponent<Selex>();
        var listOfGameObjects = new List<List<GameObject>>();
        while (selexObj.ParentCell != null)
        {
            var parentCell = selexObj.ParentCell;
            var parentSelexCell = parentCell.GetComponent<Selex>();
            listOfGameObjects.Add(parentSelexCell.AllTopologySelections);
            listOfGameObjects.Add(parentSelexCell.AllAttributeSelections);
            listOfGameObjects.Add(parentSelexCell.AllGroupsSelections);
            
            // Re-assign to parent of selex obj if such object exists
            selexObj = selexObj.ParentCell.GetComponent<Selex>();
        }
        // Now add the things that are just on the object
        listOfGameObjects.Add(selexObj.AllTopologySelections);
        listOfGameObjects.Add(selexObj.AllAttributeSelections);
        listOfGameObjects.Add(selexObj.AllGroupsSelections);
        

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
                GroupSelection(selector);
                break;
            case "TopologySelectorGO":
                TopologySelection(selector);
                break;
            case "AttributeSelectorGO":
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
        var startpointObjects = new List<GameObject>();
        startpointObjects.AddRange(_objectSelectionHandler.currentSelection);

        foreach (var startpointObject in startpointObjects)
        {
            var activeDropdownSelection = topologyObject.GetComponent<TMP_Dropdown>();
            if (!activeDropdownSelection) throw new InvalidOperationException("Topology object does not have a TMP Dropdown component attached!");
            string selectionName = activeDropdownSelection.options[activeDropdownSelection.value].text;
        
            var startpointShape = startpointObject.GetComponent<Shape>();
            List<GameObject> selectedShapes = new List<GameObject>();
            switch (selectionName)
            {
                case "child()":
                    _objectSelectionHandler.currentSelection.Remove(startpointObject);
                    selectedShapes = startpointShape.children;
                    break;
                case "parent()":
                    _objectSelectionHandler.currentSelection.Remove(startpointObject);
                    selectedShapes.Add(startpointShape.parent);
                    break;
                case "descendant()":
                    _objectSelectionHandler.currentSelection.Remove(startpointObject);
                    selectedShapes = RecursivelyGetDescendants(startpointShape);
                    break;
                case "root()":
                    selectedShapes.Add(GameObject.FindWithTag("Root"));
                    break;
                case "neighbour()":
                    _objectSelectionHandler.currentSelection.Remove(startpointObject);
                    selectedShapes = startpointShape.neighbours;
                    break;
                case "contained()":
                    // This will essentially have to check the borders of every gameObject under root
                    // And make sure they are within the input's size/borders
                    break;
                default:
                    _objectSelectionHandler.SelectEverything();
                    break; 
            }

            _objectSelectionHandler.currentSelection.AddRange(selectedShapes);
        }
        
    }

    private List<GameObject> RecursivelyGetDescendants(Shape shape)
    {
        var children = shape.children;
        var allObjs = new List<GameObject> {shape.gameObject};
        foreach (var child in children)
        {
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
        
        var attributeName = attributeNameDropdown.options[attributeNameDropdown.value].text;
        var operatorValue = operatorDropdown.options[operatorDropdown.value].text;
        switch (attributeName)
        {
            case "isOdd()":
                _objectSelectionHandler.Attr_IsOdd();
                break;
            case "isEven()":
                _objectSelectionHandler.Attr_IsEven();
                break;
            case "isEmpty()":
                _objectSelectionHandler.Attr_IsEmpty();
                break;
            case "label":
                _objectSelectionHandler.Attr_Label(operatorValue, valueField.text);
                break;
            case "idx":
                _objectSelectionHandler.Attr_Idx(operatorValue, valueField.text);
                break;
            case "pattern()":
                var patternObj = attrSelection.PatternPanel;
                _objectSelectionHandler.Attr_Pattern(patternObj);
                break;
            case "rowIdx":
                _objectSelectionHandler.Attr_RowIdx();
                break;
            case "colIdx":
                _objectSelectionHandler.Attr_ColIdx();
                break;
            default:
                Debug.Log("This should not happen, probably");
                break;
        }
        

    }
    
    /*
     * Group Selection
     */
    private void GroupSelection(GameObject selector)
    {
        var dropdown = selector.GetComponent<TMP_Dropdown>();
        var dropdownValue = dropdown.options[dropdown.value].text;
        switch (dropdownValue)
        {
            case "groupCols()":
                _objectSelectionHandler.GroupCols();
                break;
            case "groupRows()":
                _objectSelectionHandler.GroupRows();
                break;
            case "groupRegions()":
                _objectSelectionHandler.GroupRegions();
                break;
            default:
                Debug.Log("group selector fell through without matching a case. Should not happen");
                break;
        }
    }
}
