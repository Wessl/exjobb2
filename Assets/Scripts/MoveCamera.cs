using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 panOrigin;
    private float cameraZDist;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        cameraZDist = mainCam.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            panOrigin = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraZDist));
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            Vector3 difference = panOrigin - mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cameraZDist));
            mainCam.transform.position += difference;
        }
        
        // Scrolling / zooming
        float scrollDelta = Input.mouseScrollDelta.y;
        if (!scrollDelta.Equals(0f))
        {
            // Apply scroll delta, with less effect when near closest possible point to enable greater control in high zoom
            cameraZDist -= Input.mouseScrollDelta.y * cameraZDist/(16);
            // Start timer
        }

        mainCam.transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, cameraZDist);

        // If we try to zoom closer than orthogSize of 1, just say "no thank you" and stay at 1.
        if (cameraZDist >= -1)
        {
            cameraZDist = -1;
        } 
    }
}
