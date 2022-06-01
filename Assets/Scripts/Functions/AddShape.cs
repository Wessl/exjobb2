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
    [SerializeField] private TMP_Dropdown shapeTypeDropdown;
    [SerializeField] private TMP_InputField randomizationInputField;
    [SerializeField] private List<GameObject> premadeShapeTypes;

    [SerializeField] private Material temporaryMat;

    /*
     * adds a 2D construction shape to another 2D construction shape. Parameter "la" specifies the label of the new
    shape, parameters "cx, cy, w, h" specify the center point and size, parameter "offset" the relative depth with 
    respect to its parent, and parameter "visible" controls the visibility of the shape. The last two parameters are optional.
     */
    public void Execute(SelectionHandler objectSelectionHandler)
    {
        var currentlySelected = objectSelectionHandler.currentSelection;
        var newlySelected = new List<GameObject>();
        foreach (var selected in currentlySelected)
        {
            // Before we do anything, check the randomization factor and decide if we're even gonna add the current one
            if (!DetermineRandomizationResult()) continue;
            // If the object we are creating does not have a predefined mesh, make it
            var meshName = shapeTypeDropdown.options[shapeTypeDropdown.value].text;
            var shapeType = selected.GetComponent<Shape>().currentType;
            if (meshName.Equals("Default"))
            {
                CreateMesh(selected, newlySelected, shapeType);
            }
            else
            {
                var activeMesh = premadeShapeTypes[shapeTypeDropdown.value - 1];
                SpawnExistingMesh(selected, activeMesh, newlySelected, shapeType);
            }
        }
        // Assign parent, children, neighbours to each newly created object (all contained in newlySelected)
        AssignShapeRelationships(newlySelected);
        objectSelectionHandler.currentSelection = newlySelected;
    }

    private bool DetermineRandomizationResult()
    {
        var value = float.Parse(randomizationInputField.text != "" ? randomizationInputField.text : 1.ToString());
        return (Random.Range(0f, 1f) <= value);
    }

    private void AssignShapeRelationships(List<GameObject> newlyCreated)
    {
        foreach (var newObj in newlyCreated)
        {
            var newShape = newObj.GetComponent<Shape>();                    // Can this ever be null?
            newShape.Start();                                               // Essentially initializes it
            newShape.parent = newObj.transform.parent.gameObject;           // Set the parent
            newShape.parent.GetComponent<Shape>().children.Add(newObj);     // Update the parent's children
            newShape.children = null;
            var potentialNeighbours = newShape.parent.GetComponentsInChildren<Shape>();
            foreach (var potentialNeighbour in potentialNeighbours)
            {
                if (potentialNeighbour.gameObject != newShape.parent)
                {
                    newShape.neighbours.Add(potentialNeighbour.gameObject);
                }
            }
        }
    }

    private void SpawnExistingMesh(GameObject parentObj, GameObject activeMesh, List<GameObject> newlySelected, Shape.ShapeType shapeType)
    {
        // Parse numerical values from strings
        float width = float.Parse(inputWidth.text);
        float height = float.Parse(inputHeight.text);
        float cx = float.Parse(centerX.text) + parentObj.transform.position.x + width/2;
        float cy = float.Parse(centerY.text) + parentObj.transform.position.y + height/2;
        GameObject newShape;
        if (shapeType == Shape.ShapeType.Construction)
        {
            Debug.Log("spawning in to " + cx + ", " + cy);
            newShape = Instantiate(activeMesh, new Vector3(cx, cy, 0), Quaternion.identity);
            newShape.transform.parent = parentObj.transform;
            newShape.AddComponent<Shape>();
            // Finally add the finished shape to the list of currently selected objects
            newlySelected.Add(newShape);
        }
        else
        {
            newShape = parentObj;
            parentObj.GetComponent<Shape>().currentType = Shape.ShapeType.Construction; // Change from being virtual to construction?
        }
        // Make sure it gets a label
        newShape.GetComponent<Shape>().Labels.Add(label.text);

        var originalShapeSize = FindTotalMeshSize(newShape);
        // Potential bug waiting to happen... completely flat object on one axis will cause error
        newShape.transform.localScale = new Vector3(width / originalShapeSize.x, height / originalShapeSize.y, 1);
        
        // Shape extent setup
        newShape.GetComponent<Shape>().SetupSizeExtent(new Vector2(width, height));
    }

    private void CreateMesh(GameObject parentObj, List<GameObject> newlySelected, Shape.ShapeType shapeType)
    {
        Mesh mesh = new Mesh();
        // First parse numbers from strings
        float width = float.Parse(inputWidth.text);
        float height = float.Parse(inputHeight.text);
        float cx = float.Parse(centerX.text) + parentObj.transform.position.x + width/2;
        float cy = float.Parse(centerY.text) + parentObj.transform.position.y + height/2;
        // Just create a quad
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width / 2, -height / 2, 0),
            new Vector3(width / 2, -height / 2, 0),
            new Vector3(-width / 2, height / 2, 0),
            new Vector3(width / 2, height / 2, 0)
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
        
        // The shape created by AddShape should end up underneath the parent if parent is Construction type
        GameObject newShape;
        if (shapeType == Shape.ShapeType.Construction)
        {
            newShape = new GameObject(label.text);
            newShape.transform.position = new Vector3(cx, cy, 0);
            newShape.AddComponent<Shape>();
            newShape.transform.parent = parentObj.transform;
            // Finally add the finished shape to the list of currently selected objects
            newlySelected.Add(newShape);
        }
        // Else, it's virtual and it should take the place of the virtual shape
        else
        {
            newShape = parentObj;
            parentObj.GetComponent<Shape>().currentType = Shape.ShapeType.Construction; // Change from being virtual to construction?
        }
        // Make sure it gets a label
        newShape.GetComponent<Shape>().Labels.Add(label.text);
        
        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;
        newShape.GetComponent<MeshRenderer>().material = temporaryMat;
        
        // Shape extent setup
        newShape.GetComponent<Shape>().SetupSizeExtent(new Vector2(width, height));
    }
    
    EventSystem _eventSystem;
 
    void Start ()
    {
        _eventSystem = EventSystem.current;
         
    }

    private Vector2 FindTotalMeshSize(GameObject parentObj)
    {
        var meshfilters = parentObj.GetComponentsInChildren<MeshFilter>();
        Vector2 largestThusFar = new Vector2();
        foreach (var meshfilter in meshfilters)
        {
            if (Mathf.Abs(largestThusFar.x) < Mathf.Abs(meshfilter.mesh.bounds.size.x))
            {
                largestThusFar.x = meshfilter.mesh.bounds.size.x;
            }
            if (Mathf.Abs(largestThusFar.y) < Mathf.Abs(meshfilter.mesh.bounds.size.y))
            {
                largestThusFar.y = meshfilter.mesh.bounds.size.y;
            }
        }
        return largestThusFar;
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
