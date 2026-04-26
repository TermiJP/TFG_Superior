using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using NUnit.Framework.Internal;
using System.IO.Compression;
using System;

public class DesordenaPalabras : MonoBehaviour
{
    [Header("Referancias")]
    [SerializeField] private TMP_Text palabra1;
    [SerializeField] private TMP_Text palabra2;
    [SerializeField] private TMP_Text palabra3;
    public CompruebaInputs compruebaInputs;

    private void Start()
    {
        //Referenciar todo lso textos 
        palabra1 = GameObject.Find("Text_Palabra1").GetComponent<TMP_Text>();
        palabra2 = GameObject.Find("Text_Palabra2").GetComponent<TMP_Text>();
        palabra3 = GameObject.Find("Text_Palabra3").GetComponent<TMP_Text>();
        compruebaInputs = GameObject.Find("MiniGamesManager").GetComponent<CompruebaInputs>();
        
        //StartGame();
    }

    void StartGame()
    {
        palabra1.text = DesordenarPalabra(compruebaInputs.stringWord1);
        palabra2.text = DesordenarPalabra(compruebaInputs.stringWord2);
        palabra3.text = DesordenarPalabra(compruebaInputs.stringWord3);
    }

    


    public String DesordenarPalabra(string palabra)
    {
        string resultado;

        do
        {
            char[] letras = palabra.ToCharArray();

            for (int i = letras.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);

                char temp = letras[i];
                letras[i] = letras[j];
                letras[j] = temp;
            }

            resultado = new string(letras);

        } while (resultado == palabra);

        return resultado;
    }
}
