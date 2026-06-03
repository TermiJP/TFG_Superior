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

    [SerializeField] public NetworkList<FixedString64Bytes> occupiedContinents;
    [SerializeField] private List<string> occupiedContinentsDebug = new List<string>();

    private PlayerPCs player;


    void Awake()
    {
        Instance = this;
        // Inicializar aquí, no en Start
        occupiedContinents = new NetworkList<FixedString64Bytes>();
    }

    private void Start()
    {

        if (NetworkManager.Singleton.IsServer)
        {
            GetComponent<NetworkObject>().Spawn();
        }

        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();
    }


    [Rpc(SendTo.Server)]
    public void TryPlacePCRpc(string continentName)
    {
        Debug.LogWarning("Estoy intentando " + continentName);

        if (!allContinents.Contains(continentName))
        {
            Debug.LogWarning($"{continentName} no existe en el mapa.");
            return;
        }

        if (occupiedContinents.Contains(continentName))
        {
            Debug.LogWarning($"{continentName} ya tiene un PC.");
            StartCoroutine(StartMinigameNextFrame());
            return;
        }

        occupiedContinents.Add(continentName);
        // Avisa a todos que actualicen su lista debug
        SyncDebugListRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void SyncDebugListRpc()
    {
        occupiedContinentsDebug.Clear();
        foreach (var c in occupiedContinents)
            occupiedContinentsDebug.Add(c.ToString());
    }


    private IEnumerator StartMinigameNextFrame()
    {
        yield return new WaitForEndOfFrame(); // espera 1 frame
        //player.StartMinigame();
    }
}