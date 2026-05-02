using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class HackManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] public Canvas ventana;
    public String nameInput;
    public String countrieinpu;
    [SerializeField] public TMP_InputField inputName;
    [SerializeField] public TMP_InputField inputCountrie;
    public List<GameObject> Countries;

    private void Awake()
    {
        ventana  = GameObject.Find("Hacking_canvas").GetComponent<Canvas>();
       
        ventana.enabled = false;
    }


    void AbrirDesktop()
    {
        ventana.enabled = true;
    }

    void ExitDesktop()
    {
        ventana.enabled = false;
    }

    public void CheckCountrie()
    {
        string countrie = inputCountrie.text;

        GameObject foundCountry = null;

        foreach (GameObject countryObj in Countries)
        {
            if (countryObj.name == countrie)
            {
                foundCountry = countryObj;
                Debug.Log("" + foundCountry.name);
                break;
            }else
            {
                Debug.Log("Aqui no hya mierda");
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
            Debug.Log("Encontre el pais");
            NameServerPC( nameInput ,foundCountry);
        }
    }

    public void NameServerPC( string name , GameObject obj)
    {
        
        obj.name = name;
        
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

    public void BuscarhackingCanvas()
    {
        ventana = GameObject.Find("Hacking_canvas").GetComponent<Canvas>();
        if (ventana == null) Debug.Log("No esta el hacking"); 
    }

}
