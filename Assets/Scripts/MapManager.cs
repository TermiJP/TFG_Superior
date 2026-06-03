using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    public static MapManager Instance;

    [SerializeField]
    private List<string> allContinents = new List<string>()
    {
        "Europe", "Asia", "Africa", "America", "Oceania"  // añade los tuyos
    };

    [SerializeField]public NetworkList<FixedString64Bytes> occupiedContinents;

    private PlayerPCs player;
    

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();
    }

    [Rpc(SendTo.Server)]
    public void TryPlacePCRpc(string continentName)
    {
        Debug.LogWarning("Estoy intentando" + continentName);
        // 1. ¿El continente existe?
        if (!allContinents.Contains(continentName))
        {
            Debug.LogWarning($"{continentName} no existe en el mapa.");
            
        }

        // 2. ¿Ya está ocupado?
        if (occupiedContinents.Contains(continentName))
        {
            
            Debug.LogWarning($"{continentName} ya tiene un PC.");
            StartCoroutine(StartMinigameNextFrame());
            
        }

        // 3. Ocuparlo
        occupiedContinents.Add(continentName);
        
       
    }

    

    private IEnumerator StartMinigameNextFrame()
    {
        yield return new WaitForEndOfFrame(); // espera 1 frame
        player.StartMinigame();
    }
}