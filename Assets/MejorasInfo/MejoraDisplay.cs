using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MejoraDisplay : NetworkBehaviour, IPointerClickHandler
{
    public MejoraData mejoraData;
    private PlayerPCs player;

    [Header(" References ")]
    public Image spriteDisplay;
    public TMP_Text textDisplayName;
    public TMP_Text textDisplayCost;
    public TMP_Text textDisplayDescription;
    public Image Highlight;
    public GameObject RectSell;

    private void Awake()
    {
        
    }
    void Start()
    {
        spriteDisplay.sprite = mejoraData.sprite;
        textDisplayName.text = "" + mejoraData.NameMejora;
        textDisplayCost.text = "" + mejoraData.Cost.ToString();
        textDisplayDescription.text = "" + mejoraData.Description;
        //------------------------------------------------------------
        if (!IsOwner)
        {
            player = GameObject.Find("PLAYER_1").GetComponent<PlayerPCs>();
        } else
        {
            player = GameObject.Find("PLAYER_2").GetComponent<PlayerPCs>();
        }
         Debug.Log(" soy " + player.name);
        //------------------------------------------------------------

        Highlight.enabled = false;
        RectSell.SetActive(false);
    }


    


    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                ActivarHighlight();
                Comprar();
                break;
            case PointerEventData.InputButton.Right:
                ActivarRectSell();
                break;
        }
    }

    public void ActivarHighlight()
    {
        if (Highlight != null)
            Highlight.enabled = true;
    }

    public void ActivarRectSell()
    {
        if (RectSell != null){}
            RectSell.SetActive (true);
    }

    public void Comprar()
    {
        player.ComprarHabilidad(mejoraData);
    }
}
