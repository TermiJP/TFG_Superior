using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    public static MapManager Instance;

    // Guarda qué continentes ya tienen un PC
    [SerializeField] public Dictionary<string, bool> occupiedContinents = new Dictionary<string, bool>();
    private PlayerPCs player;
    // Se activa cuando alguien intenta colocar en un continente ocupado
    public bool continentAlreadyOccupied = false;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();
    }

    public bool TryPlacePC(string continentName)
    {
        if (occupiedContinents.ContainsKey(continentName) && occupiedContinents[continentName])
        {
            continentAlreadyOccupied = true;  
            Debug.LogWarning($"{continentName} ya tiene un PC.");
            StartCoroutine(StartMinigameNextFrame());
            return false;
        }

        occupiedContinents[continentName] = true;
        continentAlreadyOccupied = false;
        return true;
    }

    public void RemovePC(string continentName)
    {
        if (occupiedContinents.ContainsKey(continentName))
            occupiedContinents[continentName] = false;
    }

    private IEnumerator StartMinigameNextFrame()
    {
        yield return new WaitForEndOfFrame(); // espera 1 frame
        player.StartMinigame();
    }
}