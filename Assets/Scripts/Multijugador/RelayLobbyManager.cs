using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using UnityEngine;

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class RelayLobbyManager : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();

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

        return lobby.LobbyCode;
    }

    // CLIENTE
    public async Task JoinLobby(string lobbyCode)
    {
        Lobby lobby =
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

        string joinCode = lobby.Data["joinCode"].Value;

        JoinAllocation allocation =
            await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetClientRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        NetworkManager.Singleton.StartClient();

        // activa la sincronización de escenas
        NetworkManager.Singleton.SceneManager.OnLoad += OnSceneLoad;

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
            NetworkManager.Singleton.SceneManager.LoadScene(
                "Game",
                LoadSceneMode.Single
            );
        }
    }
}
