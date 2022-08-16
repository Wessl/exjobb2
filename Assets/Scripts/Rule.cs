using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class Rule : MonoBehaviour
{
    [SerializeField] private GameObject[] possibleRules;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject finalizePanel;
    [SerializeField] private GameObject addNewRuleButton;
    public void AddNewRule()
    {
        // Make new rule
        var newRule = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        newRule.transform.SetParent(gameObject.transform.parent, false);
        // Remove ability to click on same one to make new rule
        var button = GetComponentInChildren<Button>();
        Destroy(button.gameObject);
        
        // Instantiate Selex cell
        InstantiateSelexCell();
    }

    private void Start()
    {
        SetUpRuleChoice();
    }

    private void SetUpRuleChoice()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions( new List<string> { "Select option from dropdown..." }  );
        dropdown.AddOptions(possibleRules.Select(o => o.name).ToList());
    }

    private void InstantiateSelexCell()
    {
        var newCell = Instantiate(possibleRules[0], transform.position, Quaternion.identity);// -1 cuz default option 
        newCell.transform.SetParent(gameObject.transform.parent, false);
        newCell.transform.SetSiblingIndex(this.transform.GetSiblingIndex());                                // Put into correct hierarchy position
        
        Destroy(this.gameObject);
    }

    public void Finalize()
    {
        finalizePanel.SetActive(true);
        addNewRuleButton.SetActive(false);
    }

    public void AutomaticFinalize()
    {
        var root = GameObject.FindGameObjectWithTag("Root");
        var belowRoot = root.transform.GetChild(0);
        var extent = belowRoot.GetComponent<Shape>().SizeExent;
        
        var wall1 =Instantiate(belowRoot.gameObject, belowRoot.position, Quaternion.identity);
        wall1.transform.Translate(new Vector3(extent.x / 2, 0, extent.x / 2));
        wall1.transform.Rotate(0,  270, 0);
        wall1.transform.SetParent(root.transform);
        
        var wall2 =Instantiate(belowRoot.gameObject, belowRoot.position, Quaternion.identity);
        wall2.transform.Translate(new Vector3(0, 0, extent.x));
        wall2.transform.Rotate(0,  180, 0);
        wall2.transform.SetParent(root.transform);
        
        var wall3 =Instantiate(belowRoot.gameObject, belowRoot.position, Quaternion.identity);
        wall3.transform.Translate(new Vector3(- extent.x / 2, 0, extent.x / 2));
        wall3.transform.Rotate(0,  90, 0);
        wall3.transform.SetParent(root.transform);
        var walls = new GameObject[] {belowRoot.gameObject, wall1, wall2, wall3};

        ConstructRoof(root, walls, extent);
    }

    private void ConstructRoof(GameObject root, GameObject[] walls, Vector2 extent)
    {
        // Get edges
        Vector3[] edgePoints = new Vector3[5];
        edgePoints[0] = new Vector3(0, extent.y, 0);
        edgePoints[1] = new Vector3(extent.x, extent.y, 0);
        edgePoints[2] = new Vector3(extent.x, extent.y, extent.x);
        edgePoints[3] = new Vector3(0, extent.y, extent.x);
        // middle point (for pyramid roof)
        edgePoints[4] = new Vector3(edgePoints[2].x / 2, extent.y * 1.5f, edgePoints[2].z / 2);
        foreach (var edge in edgePoints)
        {
            
        }
        CreateRoofMesh(extent.x/2, extent.y, walls[0].transform.position.x, walls[0].transform.position.z, edgePoints[4].z, 0);
        CreateRoofMesh(extent.x/2, extent.y, walls[1].transform.position.x, walls[1].transform.position.z, edgePoints[4].z, -90);
        CreateRoofMesh(extent.x/2, extent.y, walls[2].transform.position.x, walls[2].transform.position.z, edgePoints[4].z, -180);
        CreateRoofMesh(extent.x/2, extent.y, walls[3].transform.position.x, walls[3].transform.position.z, edgePoints[4].z, -270);
        
    }
    
    private void CreateRoofMesh(float width, float height, float centerX, float centerZ, float z, float rot)
    {
        Mesh mesh = new Mesh();
        // Just create a triangle
        Vector3[] vertices = new Vector3[3]
        {
            new Vector3(-width, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height / 2, z)
        };
        mesh.vertices = vertices;
        int[] tri = new int[3]
        {
            // clockwise
            0, 2, 1
        };
        mesh.triangles = tri;
        // Normals
        Vector3[] normals = new Vector3[3]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;
        Vector2[] uv = new Vector2[3]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1)
        };
        mesh.uv = uv;
        
        GameObject newShape;
        
        newShape = new GameObject();
        newShape.transform.position = new Vector3(centerX, height, centerZ);
        newShape.transform.Rotate(new Vector3(0,rot,0), Space.World);
        newShape.AddComponent<Shape>();
        // newShape.transform.parent = parentObj.transform;

        newShape.AddComponent<MeshFilter>();
        newShape.AddComponent<MeshRenderer>();
        newShape.GetComponent<MeshFilter>().sharedMesh = mesh;
        //newShape.GetComponent<MeshRenderer>().material = temporaryMat;
        
    }

    public void CustomFinalize()
    {
        
    }
}
