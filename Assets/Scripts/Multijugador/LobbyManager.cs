using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private GameObject hostUI;
    [SerializeField] private GameObject clientUI;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private InputField joinCodeInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button startButton;

    private string lobbyCode = "";
    private List<string> playersInLobby = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        createButton.onClick.AddListener(CreateLobby);
        joinButton.onClick.AddListener(JoinLobby);
        startButton.onClick.AddListener(StartGame);

        startButton.gameObject.SetActive(false);
    }

    // ========== HOST ==========
    public void CreateLobby()
    {
        // Generar código aleatorio
        lobbyCode = Random.Range(1000, 9999).ToString("D4");

        // Iniciar como HOST
        NetworkManager.Singleton.StartHost();

        // Mostrar UI del host
        hostUI.SetActive(true);
        clientUI.SetActive(false);
        lobbyCodeText.text = $"CÓDIGO: {lobbyCode}";
        startButton.gameObject.SetActive(true);

        Debug.Log($" Lobby creado. Código: {lobbyCode}");

        // Actualizar lista de jugadores
        UpdatePlayerList();
    }

    // ========== CLIENT ==========
    public void JoinLobby()
    {
        string code = joinCodeInput.text;

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Ingresa un código válido");
            return;
        }

        // Iniciar como CLIENT
        NetworkManager.Singleton.StartClient();

        clientUI.SetActive(true);
        hostUI.SetActive(false);

        Debug.Log($"✓ Intentando unirse con código: {code}");
    }

    // ========== INICIAR JUEGO ==========
    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.LogWarning("Solo el host puede iniciar");
            return;
        }

        Debug.Log("✓ Iniciando juego...");

        // Cargar escena del juego
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    // ========== ACTUALIZAR LISTA DE JUGADORES ==========
    private void UpdatePlayerList()
    {
        /*
        playersInLobby.Clear();
        playersInLobby.Add($"Host ({NetworkManager.Singleton.LocalClientId})");

        foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (client != NetworkManager.ServerClientId)
            {
                playersInLobby.Add($"Player {client}");
            }
        }

        playerListText.text = string.Join("\n", playersInLobby);
        */
    }

    private void Update()
    {
        // Actualizar lista cada frame (puedes optimizar esto después)
        if (NetworkManager.Singleton.IsHost)
        {
            UpdatePlayerList();
        }
    }
}