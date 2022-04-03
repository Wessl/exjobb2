using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    [SerializeField] private GameObject addShapePanel;

    private GameObject lastSelection;   // used to hide the last selection easily
    private List<Action> functions = new List<Action>();

    public void FunctionCaller(int val)
    {
        // This is tremendously ugly, but who cares.
        switch (val)
        {
            case 0:
                SelectAddShape();
                break;
        }
    }

    private void SelectAddShape()
    {
        addShapePanel.SetActive(true);
        HideLastSelection();
        lastSelection = addShapePanel;
    }

    private void HideLastSelection()
    {
        lastSelection.SetActive(false);
    }
}
