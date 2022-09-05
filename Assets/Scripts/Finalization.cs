using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneTemplate;
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
    public GameObject fancyButton;
    private GameObject[] roofButtons;
    private Camera mainCam;
    public bool camCanMove;
    private float camAroundCircleRadius = 10f;
    private Vector3 centerOfBuilding;
    public List<Material> roofMaterials;
    public GameObject customBasePanel;

    private void Start()
    {
        roofButtons = new[] {flatButton, pyramidButton, tentButton, fancyButton};
        FlipRoofButtonsActive();
        mainCam = Camera.main;
        camCanMove = false;
        customBasePanel.SetActive(false);
    }

    private void Update()
    {
        if (camCanMove)
        {
            // fancy rotation around the center of the building
            var v = camAroundCircleRadius * (Vector3.forward * Mathf.Cos(Time.time) + Vector3.right * Mathf.Sin(Time.time));
            mainCam.transform.position = v + centerOfBuilding + Vector3.up * walls[0].transform.lossyScale.y;
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
        walls = new GameObject[] {belowRoot.gameObject, wall3, wall2, wall1};   //trust the order
        FlipRoofButtonsActive();
        GameObject.FindObjectOfType<Notification>().SetNotice("Automatic Finalization complete!");
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
    public void ConstructRoofFancy()
    {
        RoofConstructor.ConstructRoof(root, walls, extent, RoofConstructor.RoofType.Fancy, roofMaterials);
    }

    public void ViewBuilding()
    {
        GameObject.FindObjectOfType<GeneralUI>().ToggleUIVisible();
        // Find center point of walls
        centerOfBuilding = new Vector3(walls.Average(o => o.transform.position.x), walls.Average(o => o.transform.position.y), walls.Average(o => o.transform.position.z));
        camAroundCircleRadius = walls.Max(o => o.transform.lossyScale.x)*2f;
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

            var A = new Vector3(endPositions[i].x / sizeDelta.x - 2.5f, 0, endPositions[i].y / sizeDelta.x - 1.5f) * scaleFactor;            // magic numbers. where from???
            var B = new Vector3(endPositions[(i + 1)].x / sizeDelta.x - 2.5f, 0, endPositions[(i+1)].y / sizeDelta.x - 1.5f) * scaleFactor;
            var AB = (B - A);
            var distance = AB.magnitude;
            var halfway = AB.normalized * distance * 0.5f;
            var angle = Vector3.Angle(Vector3.right, AB.normalized);
            if (A.z < B.z) angle = -angle;
            var wall = Instantiate(belowRoot.gameObject, halfway + A, Quaternion.Euler(0,angle + 180,0));
            // how to make the scale work
            previousScalesBefore[i / 2] = wall.transform.localScale;
            wall.transform.localScale = new Vector3(distance / belowRoot.transform.localScale.x * (extent.x/4), wall.transform.localScale.y, wall.transform.localScale.z); // why does this give the correct x scaling? idk man
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
        
        // Big thing number 2: Find the winding order of the building and adjust accordingly.
        WindingOrderAdjuster();
        GameObject.FindObjectOfType<Notification>().SetNotice("Custom finalization complete");
    }

    private void WindingOrderAdjuster()
    {
        int forwardHits = 0;
        int backwardHits = 0;
        foreach (var wall in walls)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(wall.transform.position, wall.transform.forward);
            foreach (var hit in hits)
            {
                if (walls.Contains(hit.transform.gameObject))
                {
                    // another wall was hit
                    forwardHits++;
                }
            }
            hits = Physics.RaycastAll(wall.transform.position, -1 * wall.transform.forward);
            foreach (var hit in hits)
            {
                if (walls.Contains(hit.transform.gameObject))
                {
                    // another wall was hit
                    backwardHits++;
                    break;
                }
            }
        }

        if (backwardHits > forwardHits)
        {
            // Reverse every walls facing direction
            var parentTransform = walls[0].transform.parent;
            for (int i = 0; i < walls.Length; i++)
            {
                var wall = walls[i];
                wall.transform.Rotate(Vector3.up, 180);
                // we must reverse the order of how the gameObjects are in the hierarchy, since the ordering matters later when creating a roof
            }
            walls = walls.Reverse().ToArray();
        }

        if (forwardHits == 0 && backwardHits == 0)
        {
            Debug.Log("Your facade most likely does not have a collider attached, which it should for winding order to work correctly!");
        }
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
            var wallTransformAmount = wall.transform.childCount;
            for(int y = 0; y < wallTransformAmount; y++)
            {
                var wallChild = wall.transform.GetChild(y);
                if (wallChild.gameObject == wall.gameObject) continue;
                var localScale = wallChild.transform.localScale;
                var scaleDifference = (previousScalesBefore[i].x * (1 / previousScalesAfter[i].x));
                localScale = new Vector3( scaleDifference * wallChild.transform.localScale.x , wallChild.transform.localScale.y, 1/previousScalesAfter[i].z);
                wallChild.transform.localScale = localScale;
                // Check length...
                var wallLength = wall.transform.lossyScale.x;
                var childRealLength = wallChild.transform.lossyScale.x;
                
                var og_halfwidth = wallChild.transform.lossyScale.x / wall.transform.lossyScale.x;
                
                var left = (wallChild.transform.localPosition.x - og_halfwidth + 1) / 2 ;
                var right = (wallChild.transform.localPosition.x + og_halfwidth + 1) / 2;

                if (childRealLength > wallLength)
                {
                    wallChild.gameObject.SetActive(false);
                }

                float childPosOnWall = 0;
                
                childPosOnWall = - 1; //localposition will, as long as its on the wall, be between -1 and 1

                while (childPosOnWall + ((left + ( 1 - right ) + og_halfwidth)*2*scaleDifference) < 1)
                {
                    Shape wallChildShape = wallChild.GetComponent<Shape>();
                    var newWallChild = Instantiate(wallChildShape, wallChild.transform.position, wall.transform.rotation);
                    var nwcTransform = newWallChild.transform;
                    nwcTransform.SetParent(wall.transform);
                    nwcTransform.localScale = new Vector3(wall.transform.localScale.x * newWallChild.transform.localScale.x, wall.transform.localScale.y * newWallChild.transform.localScale.y, newWallChild.transform.localScale.z);
                    nwcTransform.localPosition =
                        new Vector3(((left) * 2 + og_halfwidth) * scaleDifference, nwcTransform.localPosition.y, nwcTransform.localPosition.z);
                    var halfwidth = newWallChild.transform.lossyScale.x / wall.transform.lossyScale.x;
                    
                    newWallChild.transform.localPosition += new Vector3(childPosOnWall, 0, 0);
                    // basically adjust for whenever left is over one also for right
                    childPosOnWall += (left + ( 1 - right ) + halfwidth)*2*scaleDifference;
                    
                }
                Destroy(wallChild.gameObject);
            }
        }
    }
    
}
