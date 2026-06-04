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
    MapManager map;

    public enum Estados
    {
        Bien,
        Roto,
        Bloqueado
    }

    public Estados estadoActual;

    private void Awake()
    {
        //map = GameObject.Find("MapManager").GetComponent<MapManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
        cam = Camera.main;
        estadoActual = Estados.Bien;

        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();
        //map.TryPlacePCRpc(continentName);
       
        /*
        if (!placed)
        {
            // Continente ocupado 
            player.StartMinigame();
            Destroy(this.gameObject);

            NetworkVisibility visibility = this.gameObject.GetComponent<NetworkVisibility>();
            //visibility.ApplyVisibility(true);
        }
        */
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
