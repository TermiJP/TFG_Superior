using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_Functions : MonoBehaviour
{

    private Canvas estatsCanvas;
    public bool isInside; 
    [SerializeField] private Button estatsButton;
    //public PlayerPCs playerPCs;

    // Start is called before the first frame update
    void Start()
    {
        estatsButton.onClick.AddListener(EstadisticasButtonOnClick);
        estatsCanvas = GameObject.Find("EstatsCanvas").GetComponent<Canvas>();
        estatsCanvas.enabled = false;
        //playerPCs = GameObject.Find("MainCamera").GetComponent<PlayerPCs>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EstadisticasButtonOnClick()
    {
        if( estatsCanvas.enabled == false && isInside == false)
        {
            estatsCanvas.enabled = true;
            //playerPCs.placingPC = false;
        } else 
        {
            estatsCanvas.enabled = false;
            //playerPCs.placingPC = true;
        }
    }



}
