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
    public float cantidadPcsSinPoner = 5;
    public bool placingPC;
    public float xpOrdenaodres;
    public float _sumaxp;
    public float proteccion;
    public float Peoplehacked;
    public long Peopleinfected;
    public float peligroHacker;
    public float facilidadhackeo;
    public List< GameObject> OrdenadoresEnJuego;


    public enum ProtectionState
    {
        SinProteccion,
        Baja,
        Media,
        Alta,
    }

    public ProtectionState currentState;

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

    

    public void InitReferencias()
    {
        Debug.LogWarning("ESTAN TODOS LAS REFERENCIAS");
        cam = Camera.main;
        PC_UI = GameObject.Find("CantidadOrdenadores").GetComponent<TextMeshProUGUI>();
        XP_Display = GameObject.Find("xp").GetComponent<TextMeshProUGUI>();
        Countries_Display = GameObject.Find("countries").GetComponent<TextMeshProUGUI>();
        P_Hacked_Display = GameObject.Find("hacked").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();

        Found_Display = GameObject.Find("found").GetComponent<TextMeshProUGUI>();
        alwaysCanvas = GameObject.Find("ALWAYS_CANVAS").GetComponent<Canvas>();
        hackingWindow = GameObject.Find("Hacking_canvas").GetComponent<Canvas>();
        hackManager = GameObject.Find("VentanaHacking").GetComponent<HackManager>();

        StartCoroutine(SumarPuntos());
        enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if ( _initialized == true)
        {
            InitReferencias();
            StartCoroutine(UpdateMethod());
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
            //Debug.Log("AAAAAAAAAA");

            yield return null;
        }                            
    }

    

    void HandlePlayerInfo()
    {
        XP_Display.text = "" + xpOrdenaodres;

        if ( cantidadPcsSinPoner <= 0)
        {
            
            cantidadPcsSinPoner = 0;
            
            PC_UI.text = "" + cantidadPcsSinPoner;
            return;
        }
        
    }

    
    void HandleInputs()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Debug.Log("MiniGames ");
            StartMinigame();
        }
    }

    [Rpc(SendTo.Server)]
    public void BuildPCServerRpc(Vector3 position, ulong placerClientId)
    {
        if (position == null) Debug.LogWarning("Aqui el error");
        _initialized = true;
        if (cantidadPcsSinPoner > 0)
        {
            cantidadPcsSinPoner--;

            newPC = Instantiate(PC, position, Quaternion.identity);
            newPC.GetComponent<NetworkObject>().Spawn(true);

            // 1. Obtener NetworkVisibility
            NetworkVisibility visibility = newPC.GetComponent<NetworkVisibility>();

            if (visibility != null)
            {
                visibility.InitOwner(placerClientId);
                //    visibility.SetVisibleForOthers(true);
            }

            connected = GameObject.Find("LineCompound").GetComponent<CreateConnection>();
            placingPC = false;
            AddPC(newPC);
            Debug.Log("Esta puesto");
        }
        else return;

       

        if (cantidadPcsSinPoner <= 4)
        {
            connected.AddObject(newPC.transform);
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
        OrdenadoresEnJuego.Add( pc );
    }

    //--------------------------------------------------------------------------------

    void GainXP()
    {
        foreach (GameObject pc in OrdenadoresEnJuego)
        {
            xpOrdenaodres += _sumaxp;
        }
    }

    void PeopleInfection()
    {

    }

    IEnumerator SumarPuntos()
    {
        while (true) 
        {
            yield return new WaitForSeconds(5f); 
            GainXP();
            
        }
    }

    IEnumerator SumarInfection()
    {
        while (true)
        {
            yield return new WaitForSeconds(facilidadhackeo);
            ++Peoplehacked;
        }
       
    }

    //---------------------------------------------------------------------------------

    public void ComprarHabilidad(MejoraData mejora)
    {
        if (xpOrdenaodres >= mejora.Cost)
        {
            xpOrdenaodres -= mejora.Cost;
            mejora.Aplicar(this);
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
                proteccion = 10f;
                break;

            case ProtectionState.Baja:
                proteccion = 15f;
                break;

            case ProtectionState.Media:
                proteccion = 20f;
                break;

            case ProtectionState.Alta:
                proteccion = 25f;
                break;
 
        }
    }

    public void AbrirHacking()
    {
        hackingWindow.enabled = true;
        Debug.Log("Abrir Hack");
    }

    public void StartMinigame()
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