using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Finalization : MonoBehaviour
{
    private GameObject root;
    private GameObject[] walls;
    private Vector2 extent;
    public GameObject flatButton;
    public GameObject pyramidButton;
    public GameObject tentButton;
    private GameObject[] roofButtons;
    private Camera mainCam;
    public bool camCanMove;
    private float camAroundCircleRadius = 10f;
    private Vector3 centerOfBuilding;
    public List<Material> roofMaterials;
    public GameObject customBasePanel;
    public GameObject cube;

    private void Start()
    {
        roofButtons = new[] {flatButton, pyramidButton, tentButton};
        FlipRoofButtonsActive();
        mainCam = Camera.main;
        camCanMove = false;
        customBasePanel.SetActive(false);
    }

    private void Update()
    {
        if (camCanMove)
        {
            // instead of using vector3.forward and hte like, use the center points to rotate around that instead
            var v = camAroundCircleRadius * (Vector3.forward * Mathf.Cos(Time.time) + Vector3.right * Mathf.Sin(Time.time));
            mainCam.transform.position = v + centerOfBuilding;
            mainCam.transform.LookAt(centerOfBuilding);
            
        }
    }

    public void AutomaticFinalize()
    {
        root = GameObject.FindGameObjectWithTag("Root");
        var belowRoot = root.transform.GetChild(0);
        extent = belowRoot.GetComponent<Shape>().SizeExent;
        
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
        walls = new GameObject[] {belowRoot.gameObject, wall1, wall2, wall3};
        FlipRoofButtonsActive();
    }

    private void FlipRoofButtonsActive()
    {
        foreach (var roofButton in roofButtons)
        {
            var button = roofButton.GetComponent<Button>();
            button.interactable = !button.interactable;
        }
    }

    public void ConstructRoofFlat()
    {
        RoofConstructor.ConstructRoof(root, walls, extent, RoofConstructor.RoofType.Flat, roofMaterials);
    }
    
    public void ConstructRoofPyramid()
    {
        RoofConstructor.ConstructRoof(root, walls, extent, RoofConstructor.RoofType.Pyramid, roofMaterials);
    }
    
    public void ConstructRoofTent()
    {
        RoofConstructor.ConstructRoof(root, walls, extent, RoofConstructor.RoofType.Tent, roofMaterials);
    }

    public void ViewBuilding()
    {
        GameObject.FindObjectOfType<GeneralUI>().ToggleUIVisible();
        // Find center point of walls
        centerOfBuilding = new Vector3(walls.Average(o => o.transform.position.x), walls.Average(o => o.transform.position.y), walls.Average(o => o.transform.position.z));
        camCanMove = true;
    }

    

    public void CustomFinalizeButton()
    {
        customBasePanel.SetActive(true);
    }

    public void CustomFinalize(List<Vector2> endPositions, float scaleFactor, Vector2 sizeDelta)
    {
        // Actually do the finalization
        // Set up walls[] 
        walls = new GameObject[endPositions.Count / 2];
        Vector3[] previousScalesBefore = new Vector3[endPositions.Count / 2];
        Vector3[] previousScalesAfter = new Vector3[endPositions.Count /2];
        root = GameObject.FindGameObjectWithTag("Root");
        var belowRoot = root.transform.GetChild(0);
        extent = belowRoot.GetComponent<Shape>().SizeExent;
        // make the walls appear in these positions
        var posCount = endPositions.Count;
        for (int i = 0; i < posCount-1; i += 2)
        {
            // pixel to world space... 
            var A = new Vector3(endPositions[i].x / sizeDelta.x, 0, endPositions[i].y / sizeDelta.x) * scaleFactor;
            var B = new Vector3(endPositions[(i + 1)].x / sizeDelta.x, 0, endPositions[(i+1)].y / sizeDelta.x) * scaleFactor;
            Instantiate(cube, A, Quaternion.identity);
            Instantiate(cube, B, Quaternion.identity);
            var AB = (B - A);
            var distance = AB.magnitude;
            var halfway = AB.normalized * distance * 0.5f;
            var angle = Vector3.Angle(Vector3.right, AB.normalized);
            if (A.z < B.z) angle = -angle;
            var wall =Instantiate(belowRoot.gameObject, halfway + A, Quaternion.Euler(0,angle + 180,0));
            // how to make the scale work
            previousScalesBefore[i / 2] = wall.transform.localScale;
            wall.transform.localScale = new Vector3(distance / belowRoot.transform.localScale.x * 1.25f, wall.transform.localScale.y, wall.transform.localScale.z);
            previousScalesAfter[i / 2] = wall.transform.localScale;    // save the X scaling to later correct child scales
            wall.transform.SetParent(root.transform, true);
            walls[i / 2] = wall;
        }
        FlipRoofButtonsActive();
        roofButtons[1].SetActive(false);    // Currently not supported
        roofButtons[2].SetActive(false);    // Currently not supported
        Destroy(belowRoot.gameObject);
        
        // Here comes a big thing: Fix grids so that windows correctly tile where possible, instead of just stretching
        GridFixer(previousScalesBefore, previousScalesAfter);
    }

    private void GridFixer(Vector3[] previousScalesBefore, Vector3[] previousScalesAfter)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            // My idea right now: 
            // For each wall, get every child
            // For non grids: 
            // Just check the original length, so divide by the new scaleing to maintain OG length, if another fits at equal spacing, add it there, do this for however many necessary
            // Same basic logic should also work for grids...? I hope so
            // Also if wall is too thin, remove it from that wall
            var wall = walls[i];
            var wallMeshLength = wall.GetComponent<MeshFilter>().mesh.bounds.extents.x * 2;
            var wallsChildren = wall.GetComponentsInChildren<Shape>();  // not sure if getting shape is the right move here
            foreach (var wallChild in wallsChildren)
            {
                if (wallChild.gameObject == wall.gameObject) continue;
                var localScale = wallChild.transform.localScale;
                var scaleDifference = (previousScalesBefore[i].x * (1 / previousScalesAfter[i].x));
                localScale = new Vector3( scaleDifference * wallChild.transform.localScale.x , wallChild.transform.localScale.y, 1/previousScalesAfter[i].z);
                wallChild.transform.localScale = localScale;
                // Check length...
                var wallLength = wall.transform.lossyScale.x;
                var childRealLength = wallChild.transform.lossyScale.x;
                
                var og_halfwidth = wallChild.transform.lossyScale.x / wall.transform.lossyScale.x;
                
                var left = wallChild.transform.localPosition.x - og_halfwidth;
                var right = wallChild.transform.localPosition.x + og_halfwidth;
                Debug.Log("now handling: " + wallChild.name + " where left is " + left);

                
                if (childRealLength > wallLength)
                {
                    wallChild.gameObject.SetActive(false);
                }

                float childPosOnWall = 0;
                if (left > 0)
                {
                    childPosOnWall = -1 - left + og_halfwidth;
                }
                else
                {
                    childPosOnWall = -1 + Mathf.Abs(left) + og_halfwidth; //localposition will, as long as its on the wall, be between -1 and 1
                }

                Debug.Log("do we enter the loop? childposonwall + oghalfwidth is  " + (og_halfwidth + childPosOnWall));
                while (childPosOnWall + og_halfwidth < 1)
                {
                    var newWallChild = Instantiate(wallChild, wallChild.transform.position, wall.transform.rotation);
                    
                    newWallChild.transform.SetParent(wall.transform);
                    newWallChild.transform.localScale = new Vector3(wall.transform.localScale.x * newWallChild.transform.localScale.x, wall.transform.localScale.y * newWallChild.transform.localScale.y, newWallChild.transform.localScale.z);
                    
                    var halfwidth = newWallChild.transform.lossyScale.x / wall.transform.lossyScale.x;
                    
                    newWallChild.transform.localPosition += new Vector3(childPosOnWall, 0, 0);
                    
                    childPosOnWall += halfwidth + (1-Mathf.Abs(left)) * scaleDifference + (1-Mathf.Abs(right)) * scaleDifference;
                    
                }
                Destroy(wallChild.gameObject);
                // Now check the length of the child, and the length of the remaining bits of wall. 
                // If the remaining bits of wall is greater than twice the length of the original wall, 
                // start populating it with more children (e.g. windows) at the same distance apart
                // this also requires repositioning, i.e no longer positioning based on relative position against parent, 
                // but instead the absolute distance as originally used by the reference wall. 
            }
        }
    }
    
}
