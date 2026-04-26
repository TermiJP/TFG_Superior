using UnityEngine;
using UnityEngine.EventSystems;

public class Window_UI : MonoBehaviour , IDragHandler , IPointerDownHandler
{

    Vector3 MouseDragStartPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = Input.mousePosition - MouseDragStartPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDragStartPos = Input.mousePosition - transform.localPosition;
    }
}
