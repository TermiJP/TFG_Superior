using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Globalization;
using Unity.Netcode;

public class HackManager : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] public Canvas ventana;
    public String nameInput;
    public String countrieinpu;
    [SerializeField] public TMP_InputField inputName;
    [SerializeField] public TMP_InputField inputCountrie;
    public List<GameObject> Countries;
    public GameObject foundCountry;
    private PlayerPCs player;

    private void Awake()
    {
        ventana  = GameObject.Find("Hacking_canvas").GetComponent<Canvas>();

        //---------------------------------------------------------------------------

        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();

        //----------------------------------------------------------------------------

        ventana.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        Countries = new List<GameObject>(GameObject.FindGameObjectsWithTag("Country"));

    }

    public void AbrirDesktop()
    {
        ventana.enabled = true;
        Debug.Log("Abrir ventana");
    }

    public void ExitDesktop()
    {
        ventana.enabled = false;
        Debug.Log(" Existir ventana ");
    }

    public void CheckCountrie()
    {
        foundCountry = null;
        string countrie = inputCountrie.text;

       

        foreach (GameObject countryObj in Countries)
        {
            if (countryObj.name == countrie)
            {
                foundCountry = countryObj;
                Debug.Log("" + foundCountry.name);
                break;
            }else
            {
                Debug.Log("Aqui no hay continente");
            }
        }

        if (countrie.Length == 0)
        {
            // Enseñar error
            Debug.Log("No hay countrie");
        }
        else if (countrie.Any(char.IsLetter) && foundCountry == null)
        {
            // Enseñar error
            Debug.Log("No sirve / no esta");
        }
        else if( foundCountry != null )
        {
            Debug.Log("Encontre el continente");
            NameServerPC( nameInput ,foundCountry);
            //AQUI TENGO QUE HACER EL BUILD PC
            player.BuildPCServerRpc(foundCountry.transform.position, NetworkManager.Singleton.LocalClientId);

            player.CheckPCName(countrie);
        } else
        {
            Debug.Log("No esta countrie");
        }
    }

    public void NameServerPC( string name , GameObject obj)
    {
        
       //Cambiar nombre del hack
       
        
    }

    public void CheckNameServer()
    {
        string name = inputName.text;
        if (name.Length == 0)
        {
            // Enseñar error
            Debug.Log("No hay countrie");
        }
        else if (name.Length == 10)
        {
            return;
        } else
        {
            nameInput = name;
        }

        
    }

    private void Update()
    {
       if ( ventana == null )
       {
            Debug.Log("No esta ventanahacking");
       }
    }

}
