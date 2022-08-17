using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawOnCanvas : MonoBehaviour
{
    GraphicRaycaster raycaster;
    public Image lineImage;


    private void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
 
            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);
 
            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
                var lineImg = Instantiate(lineImage, pointerData.position, Quaternion.identity);
                var lineGO = lineImg.gameObject;
                lineGO.SetActive(true);
                lineGO.transform.SetParent(gameObject.transform, true);
            }
        }
    }
}
