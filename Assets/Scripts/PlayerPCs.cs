using System.Transactions;
using TMPro;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerPCs : NetworkBehaviour
{

    Camera cam;
    [ SerializeField ] GameObject PC;

    [Header("Datos Player")]
    public NetworkVariable<float> cantidadPcsSinPoner = new NetworkVariable<float>(5,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
    public NetworkVariable<float> xpOrdenaodres = new NetworkVariable<float>(0,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
    public NetworkVariable<float> _sumaxp = new NetworkVariable<float>(1,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
    public NetworkVariable<float> proteccion = new NetworkVariable<float>(1,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
    public float Peoplehacked;
    public long Peopleinfected;
    public NetworkVariable<float> peligroHacker = new NetworkVariable<float>(1,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);
    public float facilidadhackeo;
    
    private float _infectionTimer = 0f;
    public List< GameObject> OrdenadoresEnJuego;
    private bool _firstTimeBuild = true;


    public enum ProtectionState
    {
        SinProteccion,
        Baja,
        Media,
        Alta,
    }

    public ProtectionState currentState;

    public enum TipoMejora
    {
        MasOrdenadores,
        MasXP,
        MasProteccion,
        MasPeligroHacker,
        MasFacilidadHackeo,
    }

    public TipoMejora tipos;

    [Header("Reference")]
    [SerializeField] TMP_Text PC_UI;
    //[SerializeField] TMP_Text PC_TextAd;
    [SerializeField] TMP_Text XP_Display;
    [SerializeField] TMP_Text Countries_Display;
    [SerializeField] TMP_Text P_Hacked_Display;
    [SerializeField] TMP_Text Found_Display;
    [SerializeField] Canvas  hackingWindow;
    [SerializeField] Canvas alwaysCanvas;
    [SerializeField] private TMP_Text timerText;
    private CreateConnection connected;
    private GameObject newPC;
    public HackManager hackManager;

    [Header("Mini Games")]
    [SerializeField] Canvas window_Graph;
    [SerializeField] Canvas window_Word;
    [SerializeField] Canvas window_Text;
    

    public bool _initialized = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
       

        if (!IsServer)
        {
            
            return;
        }
        
    }


    [Rpc(SendTo.Owner)]
    public void InitReferenciasRpc()
    {
        Debug.LogWarning("InitReferencesClientRpc llamado - IsOwner: " + IsOwner + " ClientId: " + OwnerClientId);
        Debug.LogWarning("ESTAN TODOS LAS REFERENCIAS");
        cam = Camera.main;
        PC_UI = GameObject.Find("CantidadOrdenadores").GetComponent<TextMeshProUGUI>();
        XP_Display = GameObject.Find("xp").GetComponent<TextMeshProUGUI>();
        //Countries_Display = GameObject.Find("countries").GetComponent<TextMeshProUGUI>();
        P_Hacked_Display = GameObject.Find("hacked").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();

        //Found_Display = GameObject.Find("found").GetComponent<TextMeshProUGUI>();
        alwaysCanvas = GameObject.Find("ALWAYS_CANVAS").GetComponent<Canvas>();
        hackingWindow = GameObject.Find("Hacking_canvas").GetComponent<Canvas>();
        hackManager = GameObject.Find("VentanaHacking").GetComponent<HackManager>();

        var xpObj = GameObject.Find("xp");
        Debug.LogWarning("xp encontrado: " + (xpObj != null));

        var cantidadObj = GameObject.Find("CantidadOrdenadores");
        Debug.LogWarning("CantidadOrdenadores encontrado: " + (cantidadObj != null));

        StartCoroutine(UpdateMethod());
        StartCoroutine(SumarPuntos());
        enabled = false;
        
    }

    
    // Update is called once per frame
    void Update()
    {
        if ( _initialized == true)
        {
            InitReferenciasRpc();
            
            _initialized = false;
        }

       
    }

    void SelectUbicacion()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); ////Cogemos la posicion en la pantalla del mouse
            mousePos.z = 0f; //// Al tener 0 en Z va recto porque es 2D
            
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); //// Lo mismo que en 3D pero en 2D, en vez de camera forward, Z a 0 y en la posion mousePos

            //// LO QUE PASA CUANDO LE DAS CLICK AL ORDENADOR -------------------------------------------
            
            if ( hit.collider.CompareTag("Ordenador")) //// si esto es distitno a no dar nada
            {
                Debug.Log("PC");
            }
            
        }
    }

    
    IEnumerator UpdateMethod()
    {
        while (true)
        {
            SelectUbicacion();
            HandlePlayerInfo();
            HandleInputs();
            InfectOverTime();

            yield return null;
        }                            
    }

    

    void HandlePlayerInfo()
    {
        if (XP_Display != null)
            XP_Display.text = "" + xpOrdenaodres.Value.ToString();
        if (PC_UI != null)
            PC_UI.text = "" + cantidadPcsSinPoner.Value.ToString();
        Debug.Log("AAAAAAAAhh");

    }

    
    void HandleInputs()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Debug.Log("MiniGames ");
            //StartMinigame();
        }
    }

    [Rpc(SendTo.Server)]
    public void BuildPCServerRpc(Vector3 position, ulong placerClientId, string name)
    {
        if (position == null) Debug.LogWarning("Aqui el error");
        _initialized = true;
        if (cantidadPcsSinPoner.Value > 0)
        {
            cantidadPcsSinPoner.Value--;
            
            newPC = Instantiate(PC, position, Quaternion.identity);

            
            PC script = newPC.GetComponent<PC>();
            script.continentName = name;

            newPC.GetComponent<NetworkObject>().Spawn(true);
            
           

            // 1. Obtener NetworkVisibility
            NetworkVisibility visibility = newPC.GetComponent<NetworkVisibility>();

            if (visibility != null)
            {
                visibility.InitOwner(placerClientId);
                //    visibility.SetVisibleForOthers(true);
            }

            connected = GameObject.Find("LineCompound").GetComponent<CreateConnection>();
            

            AddPC(newPC);
            Debug.Log("Esta puesto");
        }
        else return;



        if (cantidadPcsSinPoner.Value <= 4)
        {
            // connected.AddObject(newPC.transform);

            // Solo resta puntos si NO es la primera vez
            if (!_firstTimeBuild)
            {
                xpOrdenaodres.Value -= 25;
            }
            else
            {
                _firstTimeBuild = false; // Marca que ya pasó la primera vez
            }
        }

    }


    public void CheckPCName(string name)
    {
        PC script = PC.GetComponent<PC>();
        Debug.LogWarning("Nombre del PC es este " + name );
        script.continentName = name;
        Debug.LogWarning("Continent name es este " + script.continentName);
    }

    void AddPC( GameObject pc )
    {
        if (IsOwner || IsServer)
        {
            OrdenadoresEnJuego.Add(pc);
            Debug.Log($"PC añadido a jugador {OwnerClientId}. Total: {OrdenadoresEnJuego.Count}");
        }
    }

    //--------------------------------------------------------------------------------

    [Rpc(SendTo.Server)]
    void GainXPRpc()
    {

        xpOrdenaodres.Value += _sumaxp.Value * OrdenadoresEnJuego.Count;
    }

    private void InfectOverTime()
    {
        _infectionTimer += Time.deltaTime;

        // Cada X segundos infecta una persona nueva
        float interval = 1f / peligroHacker.Value;

        if (_infectionTimer >= interval)
        {
            _infectionTimer = 0f;
            ++Peopleinfected;
            P_Hacked_Display.text = "" + Peopleinfected;
        }
    }

    IEnumerator SumarPuntos()
    {
        while (true) 
        {
            yield return new WaitForSeconds(5f); 
            GainXPRpc();
            
        }
    }



    //---------------------------------------------------------------------------------
    public void ComprarHabilidad(MejoraData mejora)
    {
        ComprarHabilidadServerRpc(mejora.Cost, mejora.valor, mejora.tipo);
        //mejora.Aplicar(this);
    }

    [Rpc(SendTo.Server)]
    public void ComprarHabilidadServerRpc(float coste, float valor, TipoMejora tipo)
    {
        if (xpOrdenaodres.Value >= coste)
        {
            xpOrdenaodres.Value -= coste;

            switch (tipo)
            {
                case TipoMejora.MasOrdenadores:
                    cantidadPcsSinPoner.Value += valor;
                    break;

                case TipoMejora.MasXP:
                    _sumaxp.Value += valor;
                    break;

                case TipoMejora.MasProteccion:
                    proteccion.Value += valor;
                    break;

                case TipoMejora.MasPeligroHacker:
                    peligroHacker.Value += valor;
                    break;

                default:
                    Debug.LogWarning("TipoMejora no reconocido: " + tipo);
                    break;
            }
        }
        else
        {
            Debug.Log("No tienes XP");
        }
    }

    public void UpdateProteccion( ProtectionState state)
    {
        currentState = state;

        switch (currentState)
        {
            case ProtectionState.SinProteccion:
                proteccion.Value = 10f;
                break;

            case ProtectionState.Baja:
                proteccion.Value = 15f;
                break;

            case ProtectionState.Media:
                proteccion.Value = 20f;
                break;

            case ProtectionState.Alta:
                proteccion.Value = 25f;
                break;
 
        }
    }

    public void AbrirHacking()
    {
        hackingWindow.enabled = true;
        Debug.Log("Abrir Hack");
    }

    [Rpc(SendTo.NotOwner)]
    public void StartMinigameRpc()
    {

        Instantiate(window_Graph);
        Instantiate(window_Word);
        Instantiate(window_Text);
        timerText.enabled = true;
        StartCoroutine(Countdown());

    }

    private IEnumerator Countdown()
    {
        float tiempoRestante = 35f;

        while (tiempoRestante > 0)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            yield return new WaitForSeconds(1f);
            tiempoRestante--;
        }

        timerText.text = "00:00";
        //Destroy(gameObject);
    }
}