using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Window_UI : MonoBehaviour , IDragHandler , IPointerDownHandler , IPointerEnterHandler,
    IPointerExitHandler
{

    Vector3 MouseDragStartPos;
    Vector3 original;
    
    public float factorEscala = 1.1f; // 10% más grande
    public GameObject window;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original = transform.localScale;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = original * factorEscala;
    }

    // Cuando el cursor sale del objeto
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = original;
    }


    public void ExitWindow()
    {
        Destroy(window);
    }
}
