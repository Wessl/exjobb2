using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Assignment : MonoBehaviour
{
    [SerializeField] private TMP_InputField variableNameInput; 
    [SerializeField] private TMP_InputField valueInput; 
    
    // You would have to send this value up to some state
    public void OnChangeEdit_VarName()
    {
        Debug.Log(variableNameInput.text);
    }
    public void OnChangeEdit_Value()
    {
        Debug.Log(valueInput.text);
    }
}
