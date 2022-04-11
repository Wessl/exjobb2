using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public GameObject selectionParent;  // This is set by the Selex parent cell that creates it
    [SerializeField] private GameObject addShapePanel;
    [SerializeField] private GameObject createGridPanel;

    private GameObject lastSelection;   // used to hide the last selection easily

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
        
        var selexObjParent = selectionParent.GetComponent<Selex>();
        // Now find all the things being selected by this...?
        
        //while(selexObjParent != null){
            //selexObjParent = selexObjParent.GetComponent<Selex>().ParentCell;
        //}
    }
}
