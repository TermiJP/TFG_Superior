using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MultiplayerGame.Client.UI
{
    /// <summary>
    /// Gestor de la interfaz de usuario para unirse a una lobby
    /// Proporciona un panel para ingresar el código de 4 dígitos
    /// </summary>
    public class JoinLobbyUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_InputField codeInputField;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private CanvasGroup loadingIndicator;

        [Header("Manager")]
        [SerializeField] private JoinLobbyManager joinLobbyManager;

        [Header("Settings")]
        [SerializeField] private float messageDisplayTime = 5f;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private Color warningColor = Color.yellow;

        private float errorMessageTimer;
        private bool isJoining = false;

        private void Start()
        {
            SetupUI();
            SubscribeToEvents();
        }

        private void SetupUI()
        {
            // Validar referencias
            if (codeInputField == null)
            {
                Debug.LogError("InputField no asignado en JoinLobbyUI");
                return;
            }

            // Configurar el input field para solo números
            codeInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            codeInputField.characterLimit = 4;
            codeInputField.onValueChanged.AddListener(OnCodeInputChanged);
            codeInputField.onSubmit.AddListener(_ => OnJoinButtonClicked());

            // Configurar botones
            if (joinButton != null)
            {
                joinButton.onClick.AddListener(OnJoinButtonClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelButtonClicked);
            }

            // Inicializar textos
            if (statusText != null)
                statusText.text = "Ingresa el código de 4 dígitos";

            if (errorText != null)
                errorText.text = "";

            if (loadingIndicator != null)
                loadingIndicator.alpha = 0;

            SetJoinButtonEnabled(false);
        }

        private void SubscribeToEvents()
        {
            if (joinLobbyManager != null)
            {
                joinLobbyManager.OnLobbyJoinedSuccess += HandleLobbyJoinSuccess;
                joinLobbyManager.OnLobbyJoinedFailed += HandleLobbyJoinFailed;
                joinLobbyManager.OnRelayConnectionEstablished += HandleRelayConnectionSuccess;
                joinLobbyManager.OnRelayConnectionFailed += HandleRelayConnectionFailed;
            }
        }

        private void OnDestroy()
        {
            if (joinLobbyManager != null)
            {
                joinLobbyManager.OnLobbyJoinedSuccess -= HandleLobbyJoinSuccess;
                joinLobbyManager.OnLobbyJoinedFailed -= HandleLobbyJoinFailed;
                joinLobbyManager.OnRelayConnectionEstablished -= HandleRelayConnectionSuccess;
                joinLobbyManager.OnRelayConnectionFailed -= HandleRelayConnectionFailed;
            }
        }

        private void Update()
        {
            // Manejar el temporizador de error
            if (errorMessageTimer > 0)
            {
                errorMessageTimer -= Time.deltaTime;
                if (errorMessageTimer <= 0 && errorText != null)
                {
                    errorText.text = "";
                }
            }
        }

        /// <summary>
        /// Se llama cuando el valor del input field cambia
        /// </summary>
        private void OnCodeInputChanged(string newValue)
        {
            // Habilitar el botón solo si hay 4 dígitos
            bool isComplete = newValue.Length == 4 && newValue.All(char.IsDigit);
            SetJoinButtonEnabled(isComplete && !isJoining);

            // Limpiar mensaje de error cuando el usuario empieza a escribir
            if (errorText != null && !string.IsNullOrEmpty(errorText.text))
            {
                errorText.text = "";
            }
        }

        /// <summary>
        /// Se llama cuando el usuario hace clic en el botón Unirse
        /// </summary>
        private void OnJoinButtonClicked()
        {
            if (isJoining)
                return;

            string code = codeInputField.text.Trim();

            if (code.Length != 4)
            {
                ShowError("Ingresa exactamente 4 dígitos");
                return;
            }

            isJoining = true;
            SetJoinButtonEnabled(false);
            ShowLoading(true);
            SetStatus("Buscando lobby...", warningColor);

            // Llamar al manager para unirse
            if (joinLobbyManager != null)
            {
                joinLobbyManager.JoinLobbyWithCode(code);
            }
            else
            {
                ShowError("JoinLobbyManager no está asignado");
                isJoining = false;
                ShowLoading(false);
            }
        }

        /// <summary>
        /// Se llama cuando el usuario hace clic en el botón Cancelar
        /// </summary>
        private void OnCancelButtonClicked()
        {
            if (isJoining)
            {
                ShowWarning("Operación en progreso, espera a que termine");
                return;
            }

            codeInputField.text = "";
            ClearUI();
        }

        /// <summary>
        /// Manejador para cuando se une a la lobby exitosamente
        /// </summary>
        private void HandleLobbyJoinSuccess(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            SetStatus($"¡Unido a: {lobby.Name}!", successColor);
            ShowLoading(false);
            // Aquí puedes cargar la escena del juego o cambiar de panel
            Debug.Log($"Unido a la lobby: {lobby.Name}");
        }

        /// <summary>
        /// Manejador para cuando falla la unión a la lobby
        /// </summary>
        private void HandleLobbyJoinFailed(string errorMessage)
        {
            isJoining = false;
            ShowLoading(false);
            SetJoinButtonEnabled(true);
            ShowError(errorMessage);
            SetStatus("Intenta de nuevo", warningColor);
        }

        /// <summary>
        /// Manejador para cuando se establece la conexión de Relay
        /// </summary>
        private void HandleRelayConnectionSuccess(string message)
        {
            Debug.Log(message);
            SetStatus(message, successColor);
        }

        /// <summary>
        /// Manejador para cuando falla la conexión de Relay
        /// </summary>
        private void HandleRelayConnectionFailed(string errorMessage)
        {
            isJoining = false;
            ShowLoading(false);
            SetJoinButtonEnabled(true);
            ShowError(errorMessage);
        }

        /// <summary>
        /// Muestra un mensaje de estado
        /// </summary>
        private void SetStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }
        }

        /// <summary>
        /// Muestra un mensaje de error
        /// </summary>
        private void ShowError(string message)
        {
            if (errorText != null)
            {
                errorText.text = message;
                errorText.color = errorColor;
                errorMessageTimer = messageDisplayTime;
            }
            Debug.LogError($"Error en UI: {message}");
        }

        /// <summary>
        /// Muestra un mensaje de advertencia
        /// </summary>
        private void ShowWarning(string message)
        {
            if (errorText != null)
            {
                errorText.text = message;
                errorText.color = warningColor;
                errorMessageTimer = messageDisplayTime;
            }
        }

        /// <summary>
        /// Controla la visibilidad del indicador de carga
        /// </summary>
        private void ShowLoading(bool show)
        {
            if (loadingIndicator != null)
            {
                loadingIndicator.alpha = show ? 1f : 0f;
                loadingIndicator.blocksRaycasts = show;
            }
        }

        /// <summary>
        /// Habilita o deshabilita el botón de unirse
        /// </summary>
        private void SetJoinButtonEnabled(bool enabled)
        {
            if (joinButton != null)
            {
                joinButton.interactable = enabled;
            }
        }

        /// <summary>
        /// Limpia la interfaz
        /// </summary>
        private void ClearUI()
        {
            if (statusText != null)
                statusText.text = "Ingresa el código de 4 dígitos";
            if (errorText != null)
                errorText.text = "";
            SetJoinButtonEnabled(false);
            ShowLoading(false);
            isJoining = false;
        }
    }
}
