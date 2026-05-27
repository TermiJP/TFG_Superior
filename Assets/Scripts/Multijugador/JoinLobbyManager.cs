using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace MultiplayerGame.Client
{
    /// <summary>
    /// Gestor para que el cliente se una a una lobby existente usando un código de 4 dígitos.
    /// Maneja la autenticación, unión a la lobby, y configuración de Relay.
    /// </summary>
    public class JoinLobbyManager : MonoBehaviour
    {
        [SerializeField] private UnityTransport unityTransport;
        [SerializeField] private int maxRetryAttempts = 3;
        [SerializeField] private float retryDelaySeconds = 2f;

        private string currentLobbyId;
        private string playerId;
        private Lobby joinedLobby;

        // Eventos
        public event Action<Lobby> OnLobbyJoinedSuccess;
        public event Action<string> OnLobbyJoinedFailed;
        public event Action<string> OnRelayConnectionEstablished;
        public event Action<string> OnRelayConnectionFailed;

        private void OnEnable()
        {
            if (unityTransport == null)
            {
                unityTransport = GetComponent<UnityTransport>();
            }
        }

        /// <summary>
        /// Intenta unirse a una lobby usando el código de 4 dígitos del host
        /// </summary>
        /// <param name="joinCode">Código de 4 dígitos de la lobby (ej: "1234")</param>
        public async void JoinLobbyWithCode(string joinCode)
        {
            try
            {
                // Validar el código de entrada
                if (!ValidateJoinCode(joinCode))
                {
                    OnLobbyJoinedFailed?.Invoke("El código debe contener exactamente 4 dígitos.");
                    Debug.LogError("Código de unión inválido: " + joinCode);
                    return;
                }

                Debug.Log($"Intentando unirse a la lobby con código: {joinCode}");

                // Asegurar que Unity Services está inicializado
                if (!IsUnityServicesInitialized())
                {
                    await InitializeUnityServices();
                }

                // Obtener el ID del jugador
                playerId = AuthenticationService.Instance.PlayerId;
                Debug.Log($"Player ID: {playerId}");

                // Buscar la lobby por código con reintentos
                Lobby lobby = await FindLobbyByCodeWithRetry(joinCode);

                if (lobby == null)
                {
                    OnLobbyJoinedFailed?.Invoke($"No se encontró ninguna lobby con el código: {joinCode}");
                    Debug.LogError($"No se encontró lobby con código: {joinCode}");
                    return;
                }

                currentLobbyId = lobby.Id;
                Debug.Log($"Lobby encontrada: {currentLobbyId}");

                // Unirse a la lobby
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(currentLobbyId);
                Debug.Log($"Jugador unido a la lobby: {joinedLobby.Name}");

                // Configurar la conexión de Relay
                await SetupRelayConnection(joinedLobby);

                OnLobbyJoinedSuccess?.Invoke(joinedLobby);
            }
            catch (LobbyServiceException ex)
            {
                string errorMessage = HandleLobbyServiceException(ex);
                OnLobbyJoinedFailed?.Invoke(errorMessage);
                Debug.LogError($"Error en Lobby Service: {errorMessage}");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error inesperado: {ex.Message}";
                OnLobbyJoinedFailed?.Invoke(errorMessage);
                Debug.LogError($"Error al unirse a la lobby: {ex}");
            }
        }

        /// <summary>
        /// Busca una lobby por código con sistema de reintentos
        /// </summary>
        private async Task<Lobby> FindLobbyByCodeWithRetry(string joinCode)
        {
            QueryFilter filter = new QueryFilter(
                new QueryFilter.FieldOptions { Field = QueryFilter.FieldName.Code },
                QueryFilter.OpOptions.EQ,
                joinCode
            );

            QueryOptions options = new QueryOptions { Count = 1 };

            for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
            {
                try
                {
                    var lobbies = await LobbyService.Instance.QueryLobbiesAsync(filter, options);

                    if (lobbies.Results.Count > 0)
                    {
                        return lobbies.Results[0];
                    }

                    // Si no encontró, esperar antes de reintentar
                    if (attempt < maxRetryAttempts - 1)
                    {
                        Debug.LogWarning($"Lobby no encontrada. Reintentando en {retryDelaySeconds}s... (Intento {attempt + 1}/{maxRetryAttempts})");
                        await Task.Delay((int)(retryDelaySeconds * 1000));
                    }
                }
                catch (LobbyServiceException ex)
                {
                    if (attempt == maxRetryAttempts - 1)
                    {
                        throw;
                    }

                    Debug.LogWarning($"Error buscando lobby (intento {attempt + 1}): {ex.Message}. Reintentando...");
                    await Task.Delay((int)(retryDelaySeconds * 1000));
                }
            }

            return null;
        }

        /// <summary>
        /// Configura la conexión de Relay usando los datos de la lobby
        /// </summary>
        private async Task SetupRelayConnection(Lobby lobby)
        {
            try
            {
                // Obtener el join allocation del Relay
                string relayJoinCode = lobby.Data["RelayJoinCode"].Value;
                Debug.Log($"Relay Join Code obtenido: {relayJoinCode}");

                // Obtener la allocation de Relay
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

                // Obtener la dirección IP y puerto del servidor
                string ipv4Address = allocation.RelayServer.IpV4;
                int port = allocation.RelayServer.Port;

                Debug.Log($"Conectando a Relay Server: {ipv4Address}:{port}");

                // Configurar el transporte de Unity
                SetupUnityTransport(allocation, ipv4Address, port);

                OnRelayConnectionEstablished?.Invoke($"Conectado al servidor de Relay: {ipv4Address}:{port}");
                Debug.Log("Relay configurado correctamente");
            }
            catch (RelayServiceException ex)
            {
                string errorMessage = $"Error en Relay Service: {ex.Message}";
                OnRelayConnectionFailed?.Invoke(errorMessage);
                Debug.LogError(errorMessage);
                throw;
            }
        }

        /// <summary>
        /// Configura el UnityTransport con los parámetros de Relay
        /// </summary>
        private void SetupUnityTransport(JoinAllocation allocation, string ipv4Address, int port)
        {
            if (unityTransport == null)
            {
                Debug.LogError("UnityTransport no está asignado");
                return;
            }

            // Configurar el transporte
            unityTransport.SetRelayServerData(
                new RelayServerData(allocation, "wss") // "wss" para WebSocket Secure
            );

            Debug.Log("UnityTransport configurado correctamente con datos de Relay");
        }

        /// <summary>
        /// Valida que el código sea de 4 dígitos
        /// </summary>
        private bool ValidateJoinCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            code = code.Trim();

            // Verificar que sea exactamente 4 caracteres
            if (code.Length != 4)
                return false;

            // Verificar que todos sean dígitos
            foreach (char c in code)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica si Unity Services está inicializado
        /// </summary>
        private bool IsUnityServicesInitialized()
        {
            try
            {
                return UnityServices.State == ServicesInitializationState.Initialized;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Inicializa Unity Services y autentica al jugador
        /// </summary>
        private async Task InitializeUnityServices()
        {
            try
            {
                Debug.Log("Inicializando Unity Services...");

                // Inicializar servicios
                await UnityServices.InitializeAsync();

                // Autenticar de forma anónima
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Autenticación anónima completada");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error inicializando Unity Services: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Maneja excepciones de Lobby Service
        /// </summary>
        private string HandleLobbyServiceException(LobbyServiceException ex)
        {
            return ex.Reason switch
            {
                LobbyExceptionReason.AlreadyJoined => "Ya estás unido a una lobby. Sal primero.",
                LobbyExceptionReason.LobbyNotFound => "La lobby no existe.",
                LobbyExceptionReason.LobbyFull => "La lobby está llena.",
                LobbyExceptionReason.PlayerNotFound => "El jugador no se encontró.",
                LobbyExceptionReason.ParamValidationError => "Parámetros inválidos.",
                LobbyExceptionReason.Unauthorized => "No autorizado. Verifica tu sesión.",
                _ => $"Error en la lobby: {ex.Message}"
            };
        }

        /// <summary>
        /// Abandona la lobby actual
        /// </summary>
        public async void LeaveLobby()
        {
            try
            {
                if (string.IsNullOrEmpty(currentLobbyId))
                {
                    Debug.LogWarning("No estás en ninguna lobby");
                    return;
                }

                await LobbyService.Instance.RemovePlayerAsync(currentLobbyId, playerId);
                currentLobbyId = null;
                joinedLobby = null;
                Debug.Log("Has abandonado la lobby");
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError($"Error al abandonar la lobby: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información actual de la lobby
        /// </summary>
        public Lobby GetCurrentLobby() => joinedLobby;

        /// <summary>
        /// Obtiene el ID actual de la lobby
        /// </summary>
        public string GetCurrentLobbyId() => currentLobbyId;

        /// <summary>
        /// Obtiene el ID del jugador
        /// </summary>
        public string GetPlayerId() => playerId;
    }
}
