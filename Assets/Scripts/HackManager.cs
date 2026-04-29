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
    [SerializeField] public Image ventana;
    public String nameInput;
    public List<GameObject> Countries;
    

    void AbrirDesktop()
    {
        ventana.enabled = true;
    }

    void ExitDesktop()
    {
        ventana.enabled = false;
    }

    public void CheckCountrie( string countrie)
    {
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

    public void CheckNameServer( string name )
    {
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

}
