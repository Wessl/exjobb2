using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    [SerializeField] private TMP_InputField label;
    [SerializeField] private TMP_InputField rows;
    [SerializeField] private TMP_InputField columns;
    public void Execute()
    {
        var rowsList = rows.text.Split(',');
        var colsList = rows.text.Split(',');
        // Operate on the existing input shape from here...
    }
}
