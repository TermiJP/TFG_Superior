using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class CureSystem : NetworkBehaviour
{
    [Range(0, 100)] public float cureProgress = 0f;

    [Header("Disease Stats")]
    public float peligro = 10f;        
    public float infectedPercent = 5f;  // % población mundial

    [Header("Modifiers")]
    public float globalResearchFactor = 1f;
    public float hackResistance = 0f;  

    [Header("Countries")]
    public int richCountries = 5; // países que investigan fuerte

    private PlayerPCs player;

    private void Awake()
    {
        if (!IsOwner)
        {
            player = GameObject.Find("PLAYER_1").GetComponent<PlayerPCs>();
        }
        else
        {
            player = GameObject.Find("PLAYER_2").GetComponent<PlayerPCs>();
        }
        Debug.Log(" soy " + player.name);
    }

    void Update()
    {
        UpdatePlayerInfo(player);
        float delta = CalculateCureSpeed() * Time.deltaTime;
        cureProgress += delta;

        cureProgress = Mathf.Clamp(cureProgress, 0f, 100f);
    }

    float CalculateCureSpeed()
    {
        // Base por gravedad
        float peligroFactor = peligro * 0.02f;

        // Más infectados = más presión global
        float infectionFactor = infectedPercent * 0.03f;

        // Países ricos aportan más investigación
        float countryFactor = richCountries * 0.1f;

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
        peligro = player.peligroHacker;
        infectedPercent = player.Peopleinfected;
        hackResistance = player.proteccion;
    }

}
