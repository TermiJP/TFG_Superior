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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
        cam = Camera.main;

        if (!IsOwner)
        {
            player = GameObject.Find("PLAYER_1").GetComponent<PlayerPCs>();
        }
        else
        {
            player = GameObject.Find("PLAYER_2").GetComponent<PlayerPCs>();
        }

        bool placed = MapManager.Instance.TryPlacePC(continentName);

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
