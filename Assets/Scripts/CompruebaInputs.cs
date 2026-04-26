using System.Globalization;
using UnityEngine;
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

public class CompruebaInputs : MonoBehaviour
{
    String stringCodigo;
    public String stringWord1;
    public String stringWord2;
    public String stringWord3;

    public String CodigoGood;
    public List<String> Words;
    public String Word1Good;
    public String Word2Good;
    public String Word3Good;

    private TextCreation textCrea;

    private void Start()
    {
        ObtenerTresPalabras(out stringWord1, out stringWord2, out stringWord3);
        textCrea = GameObject.Find("MiniGamesManager").GetComponent<TextCreation>();
        CodigoGood = textCrea.CodigoCorrecto;
    }

    void Update()
    {
       // CodigoGood = textCrea.CodigoCorrecto;
    }


    public void ReadCodigoString(string codigoString)
    {
        if(codigoString.Length == 0)
        {
            // Enseñar error
            Debug.Log("No hay carta");
        }
        else if (codigoString.Any(char.IsLetter))
        {
            // Enseñar error
            Debug.Log("No valen los numeros");
        }
        else
        {
            stringCodigo = codigoString;
        }

        if( stringCodigo == CodigoGood)
        {
            Debug.Log("Codigo correcto ");
        }
    }

    public void ReadPalabra1( string palabra)
    {
        if (palabra.Length == 0)
        {
            // Enseñar error
        }
        else if (palabra.Any(char.IsDigit))
        {
            // Enseñar error
        }
        else
        {
            Word1Good = palabra;
        }

        if( Word1Good == stringWord1)
        {
            Debug.Log("Palabra correcto ");
        }

    }

    public void ReadPalabra2(string palabra)
    {

        if (palabra.Length == 0)
        {
            // Enseñar error
        }
        else if (palabra.Any(char.IsDigit))
        {
            // Enseñar error
        }
        else
        {
            Word2Good = palabra;
        }

        if (Word2Good == stringWord2)
        {
            Debug.Log("Palabra correcto ");
        }

    }

    public void ReadPalabra3(string palabra)
    {

        if (palabra.Length == 0)
        {
            // Enseñar error
        }
        else if (palabra.Any(char.IsDigit))
        {
            // Enseñar error
        }
        else
        {
            Word3Good = palabra;
        }

        if (Word3Good == stringWord3)
        {
            Debug.Log("Palabra correcto ");
        }

    }


    public void ObtenerTresPalabras(out string a, out string b, out string c)
    {
        if (Words.Count < 3)
        {
            Debug.LogError("No hay suficientes palabras");
            a = b = c = null;
            return;
        }

        int i1 = UnityEngine.Random.Range(0, Words.Count);
        int i2;
        int i3;

        // Asegurar que i2 es distinto de i1
        do
        {
            i2 = UnityEngine.Random.Range(0, Words.Count);
        } while (i2 == i1);

        // Asegurar que i3 es distinto de i1 e i2
        do
        {
            i3 = UnityEngine.Random.Range(0, Words.Count);
        } while (i3 == i1 || i3 == i2);

        a = Words[i1];
        b = Words[i2];
        c = Words[i3];
    }

}
