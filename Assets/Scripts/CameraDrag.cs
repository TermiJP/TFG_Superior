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
            lastMousePosition = Input.mousePosition; //// ultima ubicacion de la camara
        }

        // Mientras mantienes presionado el botón central
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition; ////// calcula la distancia entre la primera posicion y la ultima despues del grab

            // Movimiento de la cámara (invertido para sensación natural)  ////// -delta.x y -delta.y es para que haga la distancia del raton de forma inevrsa para el efecto grab
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * dragSpeed * Time.deltaTime;

            // Mover en espacio local o mundo (elige uno)  /////transform es objeto, Translate moverlo y el move es el movimiento inverso y Space es de forma WorldSpace
            transform.Translate(move, Space.World);

            lastMousePosition = Input.mousePosition;  ///// Update de la ultima posicion para volver a calcular el siguiente grab

            //-----------------------------------------------BOUNDURIES--------------------------------------------------------

            Vector3 pos = transform.position; //// COGES SU POSICION SIEMPRE

            pos.x = Mathf.Clamp(pos.x, minX, maxX); //// PONES UNOS LIMITES DIRECTACMENTRE
            pos.y = Mathf.Clamp(pos.y, minY, maxY); //// SIN NECESIDAD DE FRENAR EL MOVIMIENTO

            transform.position = pos; //// LE DEVUELVES EL VALOR

            //----------------------------------------------UPDATE POS---------------------------------------------------

            lastMousePosition = Input.mousePosition;  ///// Update de la ultima posicion para volver a calcular el siguiente grab
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
