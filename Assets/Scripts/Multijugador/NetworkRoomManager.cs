using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkRoomManager : MonoBehaviour
{
    public static NetworkRoomManager Instance;

    [Header("Configuración")]
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private ushort serverPort = 7777;

    [Header("Estado")]
    [SerializeField] private bool conectado;
    [SerializeField] private bool dentroDeSala;

    private string codigoSalaActual;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RegistrarEventosDeRed();
    }

    private void OnDestroy()
    {
        DesregistrarEventosDeRed();
    }

    // =========================================================
    // REGISTRO DE EVENTOS
    // =========================================================

    private void RegistrarEventosDeRed()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("No existe NetworkManager en la escena.");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClienteConectado;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClienteDesconectado;
    }

    private void DesregistrarEventosDeRed()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClienteConectado;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClienteDesconectado;
    }

    // =========================================================
    // UNIRSE A SALA
    // =========================================================

    /// <summary>
    /// El cliente intenta unirse a la sala usando un código.
    /// </summary>
    /// <param name="codigoSala"></param>
    public void UnirseASala(string codigoSala)
    {
        if (string.IsNullOrEmpty(codigoSala))
        {
            Debug.LogError("El código de sala está vacío.");
            return;
        }

        codigoSalaActual = codigoSala;

        ConfigurarTransporte();

        Debug.Log("Intentando conectar a la sala: " + codigoSala);

        bool resultado = NetworkManager.Singleton.StartClient();

        if (resultado)
        {
            conectado = true;
            Debug.Log("Cliente iniciado correctamente.");
        }
        else
        {
            conectado = false;
            Debug.LogError("No se pudo iniciar el cliente.");
        }
    }

    // =========================================================
    // CONFIGURAR TRANSPORTE
    // =========================================================

    private void ConfigurarTransporte()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("No existe UnityTransport.");
            return;
        }

        transport.SetConnectionData(serverIP, serverPort);

        Debug.Log("IP configurada: " + serverIP);
        Debug.Log("Puerto configurado: " + serverPort);
    }

    // =========================================================
    // EVENTOS DE RED
    // =========================================================

    private void OnClienteConectado(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        dentroDeSala = true;

        Debug.Log("Cliente conectado al servidor.");
        Debug.Log("Entró a la sala correctamente.");

        AgregarJugador();
    }

    private void OnClienteDesconectado(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        conectado = false;
        dentroDeSala = false;

        Debug.LogWarning("Cliente desconectado del servidor.");
    }

    // =========================================================
    // AGREGAR JUGADOR
    // =========================================================

    private void AgregarJugador()
    {
        Debug.Log("Agregando jugador a la partida...");

        if (!NetworkManager.Singleton.IsClient)
        {
            Debug.LogError("El cliente no está conectado.");
            return;
        }

        Debug.Log("Jugador agregado correctamente.");
    }

    // =========================================================
    // SALIR DE LA SALA
    // =========================================================

    public void SalirDeSala()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();

            conectado = false;
            dentroDeSala = false;

            Debug.Log("Cliente desconectado manualmente.");
        }
    }

    // =========================================================
    // GETTERS
    // =========================================================

    public bool EstaConectado()
    {
        return conectado;
    }

    public bool EstaDentroDeSala()
    {
        return dentroDeSala;
    }

    public string ObtenerCodigoSala()
    {
        return codigoSalaActual;
    }
}
