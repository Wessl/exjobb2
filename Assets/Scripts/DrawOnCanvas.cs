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
    private bool clickIsDown;
    private Vector2 downPos;

    private void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
        clickIsDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            this.GetComponentInParent<ScrollRect>().enabled = false;
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
 
            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            downPos = pointerData.position;
            foreach (var result in results)
            {
                Debug.Log(result.worldPosition);
            }

        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            
            this.GetComponentInParent<ScrollRect>().enabled = true;

            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
 
            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);
            var endPos = pointerData.position;
            var dirVec = (endPos - downPos);
            var distance = dirVec.magnitude;
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
                var lineImg = Instantiate(lineImage, pointerData.position, Quaternion.identity);
                var lineGO = lineImg.gameObject;
                lineGO.SetActive(true);
                lineGO.transform.SetParent(gameObject.transform, true);
                var rect = lineGO.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(distance,10);
                rect.position -= new Vector3(dirVec.x / 2, dirVec.y / 2, 0);
                var angle = Vector3.Angle(Vector2.right, dirVec);
                if (endPos.y < downPos.y) angle = -angle;   // Vector3.angle only ever returns positive value, brute force some negatives when needed
                rect.Rotate(new Vector3(0,0,angle));    
            }
        }
    }
}

