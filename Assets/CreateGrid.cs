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
        // You kind of need some sort of reference against which to make the grid? Like length and stuff
        // So you need to find the input shape. This will depend on the selection from above. 
    }
}
