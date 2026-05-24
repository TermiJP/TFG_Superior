using Newtonsoft.Json.Bson;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2f;

    private Vector3 lastMousePosition;

    [Header("Limites de cámara")]
    public Camera cam;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleDrag();
        HandleZoom();

    }

       
     void HandleDrag()
     {
        // Cuando presionas el botón central
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition; 
        }

        
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition; 

            
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * dragSpeed * Time.deltaTime;

            
            transform.Translate(move, Space.World);

            lastMousePosition = Input.mousePosition;  

            //-----------------------------------------------BOUNDURIES--------------------------------------------------------

            Vector3 pos = transform.position; 

            pos.x = Mathf.Clamp(pos.x, minX, maxX); 
            pos.y = Mathf.Clamp(pos.y, minY, maxY); 

            transform.position = pos; 

            //----------------------------------------------UPDATE POS---------------------------------------------------

            lastMousePosition = Input.mousePosition;  
        }


     }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;

            // Limitar zoom
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

            
        }
    }

    
}
