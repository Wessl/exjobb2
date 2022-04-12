using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;


public class AddShape : MonoBehaviour
{
    [SerializeField] private TMP_InputField label;
    [SerializeField] private TMP_InputField centerX;
    [SerializeField] private TMP_InputField centerY;
    [SerializeField] private TMP_InputField inputWidth;
    [SerializeField] private TMP_InputField inputHeight;
    [SerializeField] private TMP_InputField offset;
    [SerializeField] private Toggle visible;

    /*
     * adds a 2D construction shape to another 2D construction shape. Parameter "la" specifies the label of the new
    shape, parameters "cx, cy, w, h" specify the center point and size, parameter "offset" the relative depth with 
    respect to its parent, and parameter "visible" controls the visibility of the shape. The last two parameters are optional.
     */
    public void Execute()
    {
        Mesh mesh = new Mesh();
        // First parse numbers from strings
        int cx = int.Parse(centerX.text);
        int cy = int.Parse(centerY.text);
        int width = int.Parse(inputWidth.text);
        int height = int.Parse(inputHeight.text);
        // Just create a quad
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width / 2 - cx, -height / 2 + cy, 0),
            new Vector3(width / 2 + cx, -height / 2 - cy, 0),
            new Vector3(-width / 2 - cx, height / 2 + cy, 0),
            new Vector3(width / 2 + cx, height / 2 + cy, 0)
        };
        mesh.vertices = vertices;
        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;
        // Normals
        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        // Since addShape creates a new object, let's put it underneath Root for now
        var root = GameObject.FindWithTag("Root");
        var newShape = new GameObject(label.text);
        newShape.transform.parent = root.transform;
        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;

    }
    
    EventSystem _eventSystem;
 
    void Start ()
    {
        _eventSystem = EventSystem.current;
         
    }
    
    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = _eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next!= null) {
                       
                var inputField = next.GetComponent<TMP_InputField>();
                if (inputField !=null) inputField.OnPointerClick(new PointerEventData(_eventSystem));  //if it's an input field, also set the text caret
                       
                _eventSystem.SetSelectedGameObject(next.gameObject, new BaseEventData(_eventSystem));
            }
            //else Debug.Log("next nagivation element not found");
   
        }
    }

    public string Label => label.text;
}