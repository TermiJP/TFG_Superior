using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Bson;
using System.Linq;
using NUnit.Framework.Internal;
using System.IO.Compression;

public class TextCreation : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Canvas PVPCanvas;
    
    [SerializeField] TMP_Text textPVP;
    [SerializeField] private TMP_InputField inputField;
    public string CodigoCorrecto;
    public string CodigoPersonal;
    public string textoFinal;
    public TMP_Text avisoWin;

    private void Awake()
    {
        PVPCanvas = GetComponent<Canvas>();
        inputField = GameObject.Find("InputText").GetComponent<TMP_InputField>();
        avisoWin.enabled = false;
        AbrirVentanaPvP();
        
    }

    private void Update()
    {
        CodigoPersonal = inputField.text;
        CheckAnswers();
    }

    void AbrirVentanaPvP()
    {
        //PVPCanvas.enabled = true;
        textPVP.text = GenerateRandomText(300,4);
        GenerateCodeFromText( textoFinal);
        InputFieldON("0000");
        //ElegirCodigo(GenerateRandomText());
    }

    public string GenerateCodeFromText(string textoFinal)
    {
        var numbers = textoFinal.Where(c => char.IsDigit(c)).ToList();
        var length = 4;
        var code = new char[length];

        for (int i = 0; i < length; i++)
        {
            code[i] = numbers[i];
            
        }
        CodigoCorrecto = new string(code);
        return CodigoCorrecto;
    }

    public String GenerateRandomText( int numLetters , int numNumbers)
    {
        var allChars = "ABCDEFGHIJKLMNÑOPQRSTUWXYZabcdefghijklmnñopqrstuwxyz";
        var allNumbers = "123456789";
        

        var randomText = new char[numLetters + numNumbers];

        for(int i = 0; i < numLetters; i++)
        randomText[i] = allChars[UnityEngine.Random.Range(0, allChars.Length)];

        for (int i = 0; i < numNumbers; i++)
            randomText[numLetters + i] = allNumbers[UnityEngine.Random.Range(0, allNumbers.Length)];

        randomText = randomText.OrderBy( _ => UnityEngine.Random.Range(0, randomText.Length)).ToArray();
        
        textoFinal = new string(randomText);
        return textoFinal;
    } 

    public void InputFieldON( string inputString)
    {
        inputField.text = inputString;
    }

    public void CheckAnswers()
    {
        if ( CodigoCorrecto == CodigoPersonal )
        {
            LockAll();
        }
    }

    void LockAll()
    {
        inputField.interactable = false;
        avisoWin.enabled = true;
    }
}
