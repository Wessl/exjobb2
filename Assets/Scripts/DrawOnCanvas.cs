using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Vector2 endPos;
    private List<Vector2> clickedPositions;
    public float clickPosTolerance = 20f; //pixels away from previous click
    private float determineClosurePointTol = 0.1f;
    public GameObject finishButton;
    private GameObject tempLineSegment;
    [SerializeField] private TMP_InputField scaleFactorInputField;
    [SerializeField] private GameObject hardAngleTooltip;
    [SerializeField] private TMP_Text lengthTooltip;
    private bool lockHardAngle;
    

    private void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
        clickIsDown = false;
        clickedPositions = new List<Vector2>();
        finishButton.SetActive(false);
        hardAngleTooltip.SetActive(false);
        lockHardAngle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Just display the tooltip if shift is not held BUG: right now ClosestAngle doesn't work so I'm putting this on hold for now
            // hardAngleTooltip.SetActive(true);
            
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
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            // While mouse is down, update the line segment continuously
            
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
 
            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;

            this.raycaster.Raycast(pointerData, results);
            if (tempLineSegment) Destroy(tempLineSegment);   // destroy previous one
            DisplayLineSegment(pointerData);
            
           
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
            
            if (tempLineSegment) Destroy(tempLineSegment);   // destroy previous one
            DisplayLineSegment(pointerData);
            
            
            clickedPositions.Add(downPos);
            clickedPositions.Add(endPos);
            
            // Check if this completes a closed shape
            DetermineClosure();
            
            // Hide shift tooltip
            hardAngleTooltip.SetActive(false);
            
            // un-assign previous line segment so it can't be deleted
            tempLineSegment = null;
        }
    }

    private GameObject DisplayLineSegment(PointerEventData pointerData)
    {
        endPos = pointerData.position;
        var dirVec = (endPos - downPos);
        var distance = dirVec.magnitude;
        var lineImg = Instantiate(lineImage, pointerData.position, Quaternion.identity);
        tempLineSegment = lineImg.gameObject;
        tempLineSegment.SetActive(true);
        tempLineSegment.transform.SetParent(gameObject.transform, true);
        var rect = tempLineSegment.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(distance,10);
        rect.position -= new Vector3(dirVec.x / 2, dirVec.y / 2, 0);
        var angle = Vector3.Angle(Vector2.right, dirVec);
        if (endPos.y < downPos.y) angle = -angle;   // Vector3.angle only ever returns positive value, brute force some negatives when needed
        // angle = ClosestAngle(angle);
        rect.Rotate(new Vector3(0,0,angle));
        DisplayTextToolTip(distance);
        return tempLineSegment;
    }

    void DisplayTextToolTip(float distance)
    {
        var sizeDelta = this.gameObject.GetComponent<RectTransform>().sizeDelta;
        var scaleFactor = 1f;
        if (scaleFactorInputField.text != "")
        {
            scaleFactor = float.Parse(scaleFactorInputField.text);
        }
        lengthTooltip.text = (distance * scaleFactor/ sizeDelta.x).ToString("0.00");

    }

    // Closest angle doesn't really work and I don't have the time to fix it rn 
    float ClosestAngle(float angle)
    {
        // Not sure if this is the most efficient but w/e
        float[] angleIncrements = new float[] {-180f, -135f, -90f, -45f, 0f, 45f, 90f, 135f, 180f};
        for (int i = 0; i < angleIncrements.Length; i++)
        {
            if (Mathf.Abs(Mathf.Abs(angle) - Mathf.Abs(angleIncrements[i])) < 45 * 0.5f)
            {
                return angleIncrements[i];
            }
        }

        return angle;
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
        var scaleFactor = 1f;
        if (scaleFactorInputField.text != "")
        {
            scaleFactor = float.Parse(scaleFactorInputField.text);
        }
        // this.gameObject.SetActive(false);
        var sizeDelta = this.gameObject.GetComponent<RectTransform>().sizeDelta;
        GameObject.FindObjectOfType<Finalization>().CustomFinalize(clickedPositions, scaleFactor, sizeDelta);
    }

    public void ResetCanvas()
    {
        // Remove everything
        clickedPositions = new List<Vector2>();
        var oldImages = this.GetComponentsInChildren<Image>().ToList();
        oldImages.RemoveAt(0);
        foreach (var oldImage in oldImages)
        {
            Destroy(oldImage);
        }
        finishButton.SetActive(false);
        clickIsDown = false;
    }
}

