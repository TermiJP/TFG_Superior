using UnityEngine;
using Unity.Netcode;
using MultiplayerGame.Client;

namespace MultiplayerGame.Client.Example
{
    /// <summary>
    /// Ejemplo de GameManager que integra el sistema de unión a lobby
    /// Este script muestra cómo manejar el flujo después de unirse a la lobby
    /// </summary>
    public class ClientGameManager : MonoBehaviour
    {
        [SerializeField] private JoinLobbyManager joinLobbyManager;
        [SerializeField] private NetworkManager networkManager;
        
        [SerializeField] private string gameSceneName = "GameScene";
        [SerializeField] private string menuSceneName = "MenuScene";

        private bool isConnected = false;

        private void Start()
        {
            ValidateReferences();
            SubscribeToNetworkEvents();
        }

        private void ValidateReferences()
        {
            if (joinLobbyManager == null)
            {
                Debug.LogError("JoinLobbyManager no asignado en ClientGameManager");
            }

            if (networkManager == null)
            {
                networkManager = FindObjectOfType<NetworkManager>();
            }
        }

        private void SubscribeToNetworkEvents()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback += OnClientConnected;
                networkManager.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }

        /// <summary>
        /// Se llama cuando el cliente se conecta al servidor
        /// </summary>
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Cliente {clientId} conectado al servidor");
            isConnected = true;
            
            // Aquí puedes hacer lo siguiente:
            // - Cargar la escena del juego
            // - Instanciar el jugador
            // - Sincronizar el estado del juego
            
            LoadGameScene();
        }

        /// <summary>
        /// Se llama cuando el cliente se desconecta del servidor
        /// </summary>
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"Cliente {clientId} desconectado");
            isConnected = false;
            
            // Volver al menú si se desconecta
            ReturnToMenu();
        }

        /// <summary>
        /// Carga la escena del juego
        /// </summary>
        private void LoadGameScene()
        {
            Debug.Log($"Cargando escena: {gameSceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// Vuelve al menú principal
        /// </summary>
        private void ReturnToMenu()
        {
            Debug.Log($"Volviendo al menú: {menuSceneName}");
            
            // Abandonar la lobby si estamos en una
            if (joinLobbyManager != null)
            {
                joinLobbyManager.LeaveLobby();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }

        /// <summary>
        /// Desconecta el cliente del servidor
        /// </summary>
        public void DisconnectFromServer()
        {
            if (networkManager != null && isConnected)
            {
                networkManager.Shutdown();
            }
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback -= OnClientConnected;
                networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
    }
}
