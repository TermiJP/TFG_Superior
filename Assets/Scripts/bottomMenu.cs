using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class bottomMenu : MonoBehaviour , IPointerEnterHandler
{
    public Image selected;

    void Start()
    {
       selected.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       selected.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected.enabled = false;
    }
}
