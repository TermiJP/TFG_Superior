using UnityEngine;
using UnityEngine.UI;

public class PC : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void OnMouseExit()
    {
        canvas.enabled = false;
    }

    private void OnMouseEnter()
    {
        canvas.enabled = true;
    }
}
