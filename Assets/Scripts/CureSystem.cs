using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CureSystem : NetworkBehaviour
{
    [Range(0, 100)] public float cureProgress = 0f;

    [Header("Disease Stats")]
    public float peligro;        
    public float infectedPercent;  // % población mundial

    [Header("Modifiers")]
    public float globalResearchFactor = 0.05f;
    public float hackResistance = 0f;  

    [Header("Countries")]
    public int richCountries = 2; // países que investigan fuerte

    private PlayerPCs player;
    [SerializeField] Canvas aviso_lose;
    [SerializeField] Button exitBottom;
    

    private void Awake()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPCs>();
        aviso_lose.enabled = false;

    }

    void Update()
    {
        UpdatePlayerInfo(player);
        float delta = CalculateCureSpeed() * Time.deltaTime;
        cureProgress += delta;

        cureProgress = Mathf.Clamp(cureProgress, 0f, 100f);
        if( cureProgress >= 100)
        {
            aviso_lose.enabled = true;
           
        }


        
    }

    float CalculateCureSpeed()
    {
        // Base por gravedad
        float peligroFactor = peligro * 0.001f;

        // Más infectados = más presión global
        float infectionFactor = infectedPercent * 0.001f;

        // Países ricos aportan más investigación
        float countryFactor = richCountries * 0.02f;

        // Suma 
        float baseSpeed = peligroFactor + infectionFactor + countryFactor;

        // Aplicar factor global 
        baseSpeed *= globalResearchFactor;

        // Reducir por resistencia del virus
        baseSpeed *= (1f - hackResistance);

        return baseSpeed;
    }

    void UpdatePlayerInfo(PlayerPCs player)
    {
        peligro = player.peligroHacker.Value;
        infectedPercent = player.Peopleinfected.Value;
        hackResistance = player.proteccion.Value;
    }

    public void ExitMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
