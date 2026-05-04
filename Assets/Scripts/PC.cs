using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PC : NetworkBehaviour
{
    [SerializeField] Canvas canvas;
    private PlayerPCs player;
    Camera cam;
    public bool puesto;
    public string continentName;

    private void Awake()
    {
        if (!IsOwner)
        {
            player = GameObject.Find("PLAYER_1").GetComponent<PlayerPCs>();
        }
        else
        {
            player = GameObject.Find("PLAYER_2").GetComponent<PlayerPCs>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool placed = MapManager.Instance.TryPlacePC(continentName);
        canvas.enabled = false;
        cam = Camera.main;
        if (!placed)
        {
            // Continente ocupado 
            player.StartMinigame();
            
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
}
