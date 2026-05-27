using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private GameObject hostUI;
    [SerializeField] private GameObject clientUI;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private TMP_InputField joinCodeInput;
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
        lobbyUI.SetActive(false);
        //clientUI.SetActive(false);
        lobbyCodeText.text = "" + lobbyCode;
        startButton.gameObject.SetActive(true);
        // Agregar nombre del host a la lista
        //AgregarJugador();

        Debug.Log($" Lobby creado. Código: {lobbyCode}");

        
    }

    // ========== CLIENT ==========
    public void JoinLobby()
    {
        /*
        string code = joinCodeInput.text;

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Ingresa un código válido");
            return;
        }
        */
        // Iniciar como CLIENT
        NetworkManager.Singleton.StartClient();
        
        clientUI.SetActive(true);
        //hostUI.SetActive(false);

        Debug.Log("Entrando en Client Lobby");
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
        NetworkManager.Singleton.SceneManager.LoadScene("GamePlay",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    // ========== MÉTODOS PARA ACTUALIZAR NOMBRES ==========

    
    private void AgregarJugador( TMP_InputField nombreJugador)
    {
        /*
        if (!playersInLobby.Contains(nombreJugador))
        {
            playersInLobby.Add(nombreJugador);
            ActualizarListaJugadores();
            Debug.Log($"✓ Jugador agregado: {nombreJugador}");
        }
        */
    }

    
    private void ActualizarListaJugadores()
    {
        playerListText.text = "";

        for (int i = 0; i < playersInLobby.Count; i++)
        {
            playerListText.text += $"Jugador {i + 1}: {playersInLobby[i]}\n";
        }

        Debug.Log($"✓ Lista actualizada: {playersInLobby.Count} jugadores");
    }

    
    [Rpc(SendTo.Server)]
    private void NotificarUnionServerRpc(string nombreJugador)
    {
        // El host ejecuta esto cuando un cliente se une
        //AgregarJugador(nombreJugador);

        
        ActualizarListaClientesRpc();
    }

    
    [Rpc(SendTo.Everyone)]
    private void ActualizarListaClientesRpc()
    {
        ActualizarListaJugadores();
    }

    public void ClientJoinLobby()
    {
        string codigo = joinCodeInput.text;

        // PASO 1: Obtener datos del servidor
        //LobbyData datos = ObtenerDatosDelBackend(codigo);
        // datos.hostIP = "192.168.1.5"
        // datos.port = 7777

        // PASO 2: Configurar dónde conectarse
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //transport.SetConnectionData(datos.hostIP, (ushort)datos.port);

        // PASO 3: Conectar
        NetworkManager.Singleton.StartClient();

        // ¡LISTO! Ahora está conectado al HOST
    }
}