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

    [SerializeField] private TMP_InputField scaleXInputField;
    [SerializeField] private TMP_InputField scaleYInputField;
    [SerializeField] private TMP_InputField scaleZInputField;
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
    public void ExecuteScale(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        var scaleX = float.Parse(scaleXInputField.text);
        var scaleY = float.Parse(scaleYInputField.text);
        var scaleZ = float.Parse(scaleZInputField.text);
        // You have to get the axes of rotation and how much, then just apply it
        foreach (var currentObj in currentlySelected)
        {
            var scale = currentObj.transform.localScale;
            scale = new Vector3(scale.x * scaleX, scale.y * scaleY, scale.z * scaleZ);
            currentObj.transform.localScale = scale;
        }
    }
}
