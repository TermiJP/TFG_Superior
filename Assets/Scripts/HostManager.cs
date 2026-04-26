using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HostManager : NetworkBehaviour
{

    [ SerializeField ] private Button hostButton;
    [ SerializeField ] private Button clientButton;
    [ SerializeField ] private Button PointButton;
    //---------------------------------------------------------

    public PointManager pointManager;

    // Start is called before the first frame update
    void Start()
    {
        pointManager = GameObject.Find("DEVManager").GetComponent<PointManager>();
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
        PointButton.onClick.AddListener(PointButtonOnClick);

        //------------------------------------------------

        if ( hostButton == null)
        {
            return;
        }

        if (clientButton == null)
        {
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
        /*
        Debug.Log("Entro al host scene");
        SceneManager.LoadScene("Host");
        */
    }

    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
        
    }

    private void PointButtonOnClick()
    {
        if (IsOwner)
        {
            pointManager.puntos++;
            Debug.Log("soy player 1 y tengo " + pointManager.puntos);
        } else
        {
            pointManager.puntosClient++;
            Debug.Log("soy player 2 y tengo " + pointManager.puntosClient);
        }
        
    }
}
