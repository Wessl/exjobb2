using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawOnCanvas : MonoBehaviour
{
    GraphicRaycaster raycaster;
    public Image lineImage;
    private bool clickIsDown;
    private Vector2 downPos;
    private List<Vector2> clickedPositions;
    public float clickPosTolerance = 50f; //pixels away from previous click
    private float determineClosurePointTol = 0.1f;
    public GameObject finishButton;
    private float scaleFactor;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
        clickIsDown = false;
        clickedPositions = new List<Vector2>();
        finishButton.SetActive(false);
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
            
            // Check distances from previous click locations
            pointerData.position = LeastDistancePoint(pointerData);

            this.raycaster.Raycast(pointerData, results);

            downPos = pointerData.position;
            foreach (var result in results)
            {
                // Debug.Log(result.worldPosition);
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
            if ((pointerData.position - downPos).magnitude < determineClosurePointTol) return;  // it was just a click, not a drag
            
            // Check distances from previous click locations
            pointerData.position = LeastDistancePoint(pointerData);
            
            this.raycaster.Raycast(pointerData, results);
            var endPos = pointerData.position;
            var dirVec = (endPos - downPos);
            var distance = dirVec.magnitude;
            foreach (RaycastResult result in results)
            {
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
            clickedPositions.Add(downPos);
            clickedPositions.Add(endPos);
            
            // Check if this completes a closed shape
            DetermineClosure();
        }
        
        
    }
    Vector2 LeastDistancePoint(PointerEventData pointerData)
    {
        var lowestDistance = float.MaxValue;
        Vector2 closestPoint = pointerData.position;
        foreach (var clickPos in clickedPositions)
        {
            var distPoint = Vector2.Distance(pointerData.position, clickPos);
            if (distPoint < clickPosTolerance && distPoint < lowestDistance)
            {
                lowestDistance = distPoint;
                closestPoint = clickPos;
            }
        }

        return closestPoint;
    }

    private void DetermineClosure()
    {
        int markedPositions = 0;
        for (int i = 0; i < clickedPositions.Count; i++)
        {
            for (int y = 0; y < clickedPositions.Count; y++)
            {
                if (i == y) continue;
                if ((clickedPositions[i] - clickedPositions[y]).magnitude < determineClosurePointTol)
                {
                    markedPositions ++;
                }
            }
        }

        if (markedPositions == clickedPositions.Count && markedPositions > 2)
        {
            finishButton.SetActive(true);
        }
        else
        {
            finishButton.SetActive(false);
        }
    }

    public void ClickFinishButton()
    {
        scaleFactor = float.Parse(inputField.text);
        this.gameObject.SetActive(false);
        var sizeDelta = this.gameObject.GetComponent<RectTransform>().sizeDelta;
        GameObject.FindObjectOfType<Finalization>().CustomFinalize(clickedPositions, scaleFactor, sizeDelta);
    }
}

