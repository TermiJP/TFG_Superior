using System.Transactions;
using TMPro;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public float Peopleinfected;
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
    [SerializeField] TMP_Text PC_TextAd;
    [SerializeField] TMP_Text XP_Display;
    [SerializeField] TMP_Text Countries_Display;
    [SerializeField] TMP_Text P_Hacked_Display;
    [SerializeField] TMP_Text Found_Display;
    [SerializeField] Canvas  hackingWindow;
    [SerializeField] Canvas alwaysCanvas;
    private CreateConnection connected;
    private GameObject newPC;

    [Header("Mini Games")]
    [SerializeField] Canvas window_Graph;
    [SerializeField] Canvas window_Word;
    [SerializeField] Canvas window_Text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cam = Camera.main;
        PC_UI = GameObject.Find("CantidadOrdenadores").GetComponent<TextMeshProUGUI>();
        PC_TextAd = GameObject.Find("Placing PCs Adverstisemnet").GetComponent<TextMeshProUGUI>();
        XP_Display = GameObject.Find("xp").GetComponent<TextMeshProUGUI>();
        Countries_Display = GameObject.Find("countries").GetComponent<TextMeshProUGUI>();
        P_Hacked_Display = GameObject.Find("hacked").GetComponent<TextMeshProUGUI>();
        Found_Display = GameObject.Find("found").GetComponent<TextMeshProUGUI>();
        alwaysCanvas = GameObject.Find("ALWAYS_CANVAS").GetComponent<Canvas>();
        hackingWindow = GameObject.Find("Hacking_canvas").GetComponent<Canvas>();
        PC_TextAd.enabled = false;
        StartCoroutine(SumarPuntos());
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectUbicacion();
        BuildPC();
        HandlePlayerInfo();
        HandleInputs();

        
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

    void BuildPC()
    {
        BuildPCServerRpc();
        
    }

    void HandlePlayerInfo()
    {
        if( cantidadPcsSinPoner <= 0 )
        {
            cantidadPcsSinPoner = 0;
            return;
        }
        XP_Display.text = "" + xpOrdenaodres;
        

    }

    void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.AltGr))
        {
            placingPC = true;
            PC_TextAd.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Debug.Log("pene pene ");
            StartMinigame();
        }
    }

    [ServerRpc]
    private void BuildPCServerRpc()
    {
        if (Input.GetMouseButtonDown(0) && placingPC == true)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); ////Cogemos la posicion en la pantalla del mouse
            mousePos.z = 0f; //// Al tener 0 en Z va recto porque es 2D

            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); //// Lo mismo que en 3D pero en 2D, en vez de camera forward, Z a 0 y en la posion mousePos

            //// LO QUE PASA CUANDO LE DAS CLICK DERECHO -------------------------------------------

            Debug.Log("Poniendo PC");

            if (cantidadPcsSinPoner > 0)
            {
                cantidadPcsSinPoner--;

                newPC = Instantiate(PC, mousePos, Quaternion.identity);
                newPC.GetComponent<NetworkObject>().Spawn(true);
                connected = GameObject.Find("LineCompound").GetComponent<CreateConnection>();
                placingPC = false;
                PC_TextAd.enabled = false;
                AddPC(newPC);

            }
            else return;
            PC_UI.text = "" + cantidadPcsSinPoner;
            if (cantidadPcsSinPoner <= 4)
            {
                connected.AddObject(newPC.transform);
            }

        }
    }

    void AddPC( GameObject pc )
    {
        OrdenadoresEnJuego.Add( pc );
    }

    void GainXP()
    {
        foreach (GameObject pc in OrdenadoresEnJuego)
        {
            xpOrdenaodres += _sumaxp;
        }
    }

    IEnumerator SumarPuntos()
    {
        while (true) 
        {
            yield return new WaitForSeconds(2f); 
            GainXP();
            
        }
    }

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
        
    }

    public void StartMinigame()
    {
        Instantiate(window_Graph);
        Instantiate(window_Word);
        Instantiate(window_Text);

        Debug.Log("Le estoy dando ");
    }
}