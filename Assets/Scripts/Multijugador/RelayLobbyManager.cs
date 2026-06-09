using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Collections;

public class RelayLobbyManager : MonoBehaviour
{
    [SerializeField]TMP_Text nombrePlayer;
    [SerializeField] TMP_Text codigoSala;


    async void Start()
    {
        await UnityServices.InitializeAsync();
        nombrePlayer.enabled = false;
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    // HOST
    public async Task<string> CreateLobby()
    {
        Allocation allocation =
            await RelayService.Instance.CreateAllocationAsync(4);

        string joinCode =
            await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode",
                    new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode)
                }
            }
        };

        Lobby lobby =
            await LobbyService.Instance.CreateLobbyAsync(
                "MiLobby",
                4,
                options
            );

        NetworkManager.Singleton.StartHost();

        Debug.Log("Lobby creado: " + lobby.LobbyCode);
        codigoSala.text = "" + lobby.LobbyCode;
        return lobby.LobbyCode;
    }

    // CLIENTE
    public async Task JoinLobby(string lobbyCode)
    {
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
        string joinCode = lobby.Data["joinCode"].Value;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetClientRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        nombrePlayer.enabled = true;
        // Suscribirse ANTES de StartClient
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            Debug.Log("Cliente aprobado por el host, ID: " + clientId);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            Debug.Log("Cliente DESCONECTADO, ID: " + clientId +
                      " Reason: " + NetworkManager.Singleton.DisconnectReason);
        };

        
        NetworkManager.Singleton.StartClient();
        Debug.Log("Cliente conectado");
        
    }

    private void OnSceneLoad(ulong clientId, string sceneName,LoadSceneMode loadSceneMode,AsyncOperation asyncOperation)
    {
        Debug.Log($"[Cliente] Cargando escena: {sceneName}");
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            StartCoroutine(LoadWhenReady());
        }
    }

    private IEnumerator LoadWhenReady()
    {
        yield return new WaitUntil(() =>
            NetworkManager.Singleton.ConnectedClients.Count >= 2
        );

        yield return new WaitForSeconds(0.5f);

        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
