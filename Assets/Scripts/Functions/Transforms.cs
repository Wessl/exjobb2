using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    [SerializeField] private TMP_InputField translateXInputField;
    [SerializeField] private TMP_InputField translateYInputField;
    [SerializeField] private TMP_InputField translateZInputField;
    public void ExecuteRotate(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        var rotX = float.Parse(rotateX.text, CultureInfo.InvariantCulture);
        var rotY = float.Parse(rotateY.text, CultureInfo.InvariantCulture);
        var rotZ = float.Parse(rotateZ.text, CultureInfo.InvariantCulture);
        // You have to get the axes of rotation and how much, then just apply it
        foreach (var currentObj in currentlySelected)
        {
            currentObj.transform.Rotate(new Vector3(rotX, rotY, rotZ),Space.World);
        }
    }
    public void ExecuteScale(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        var scaleX = float.Parse(scaleXInputField.text != "" ? scaleXInputField.text : 1.ToString(), CultureInfo.InvariantCulture);
        var scaleY = float.Parse(scaleYInputField.text != "" ? scaleYInputField.text : 1.ToString(), CultureInfo.InvariantCulture);
        var scaleZ = float.Parse(scaleZInputField.text != "" ? scaleZInputField.text : 1.ToString(), CultureInfo.InvariantCulture);
        // You have to get the axes of scale and how much, then just apply it
        foreach (var currentObj in currentlySelected)
        {
            var scale = currentObj.transform.localScale;
            scale = new Vector3(scale.x * scaleX, scale.y * scaleY, scale.z * scaleZ);
            currentObj.transform.localScale = scale;
        }
    }
    
    public void ExecuteTranslation(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        var translateX = float.Parse(translateXInputField.text != "" ? translateXInputField.text : 0.ToString(), CultureInfo.InvariantCulture);
        var translateY = float.Parse(translateYInputField.text != "" ? translateYInputField.text : 0.ToString(), CultureInfo.InvariantCulture);
        var translateZ = float.Parse(translateZInputField.text != "" ? translateZInputField.text : 0.ToString(), CultureInfo.InvariantCulture);
        // You have to get the axes of translation and how much, then just apply it
        foreach (var currentObj in currentlySelected)
        {
            var translation = new Vector3(translateX, translateY, translateZ);
            currentObj.transform.position += translation;
        }
    }
}
