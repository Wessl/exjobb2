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
            Debug.Log(v);
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

    

    public void CustomFinalize()
    {
        customBasePanel.SetActive(true);
    }
}
