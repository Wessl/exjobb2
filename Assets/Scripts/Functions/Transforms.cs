using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Transforms : MonoBehaviour
{
    /*
     * This class contains the functionality for performing transformation actions
     * Either complete transforms,
     * Or individual parts [rotation, scaling, moving]
     */
    
    [SerializeField] private TMP_InputField rotateX;
    [SerializeField] private TMP_InputField rotateY;
    [SerializeField] private TMP_InputField rotateZ;
    public void ExecuteRotate(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        var rotX = float.Parse(rotateX.text);
        var rotY = float.Parse(rotateY.text);
        var rotZ = float.Parse(rotateZ.text);
        // You have to get the axes of rotation and how much, then just apply it
        foreach (var currentObj in currentlySelected)
        {
            currentObj.transform.Rotate(new Vector3(rotX, rotY, rotZ),Space.World);
        }
    }
}
