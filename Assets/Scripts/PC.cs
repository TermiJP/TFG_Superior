using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PC : NetworkBehaviour
{
    [SerializeField] Canvas canvas;
    Camera cam;
    public string continentName;
    private PlayerPCs player;
    [SerializeField] SpriteRenderer sprite;
    [Header("PC Stats")]
    public int vida;

    public enum Estados
    {
        Bien,
        Roto,
        Bloqueado
    }

    public Estados estadoActual;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
        cam = Camera.main;
        estadoActual = Estados.Bien;

        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();

        bool placed = MapManager.Instance.TryPlacePC(continentName);

        if (!placed)
        {
            // Continente ocupado 
            player.StartMinigame();
            NetworkVisibility visibility = this.gameObject.GetComponent<NetworkVisibility>();
            //visibility.ApplyVisibility(true);
        }
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

    public void TakeDamage()
    {
        estadoActual = Estados.Roto;
        sprite.color = Color.red;
    }
}
