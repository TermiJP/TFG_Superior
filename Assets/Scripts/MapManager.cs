using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    // Guarda qué continentes ya tienen un PC
    private Dictionary<string, bool> occupiedContinents = new Dictionary<string, bool>();

    // Se activa cuando alguien intenta colocar en un continente ocupado
    public bool continentAlreadyOccupied = false;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Intenta colocar un PC en un continente.
    /// Devuelve true si se colocó, false si ya estaba ocupado.
    /// </summary>
    public bool TryPlacePC(string continentName)
    {
        if (occupiedContinents.ContainsKey(continentName) && occupiedContinents[continentName])
        {
            continentAlreadyOccupied = true;  // <-- el bool que quieres
            Debug.Log($"{continentName} ya tiene un PC.");
            return false;
        }

        occupiedContinents[continentName] = true;
        continentAlreadyOccupied = false;
        return true;
    }

    public void RemovePC(string continentName)
    {
        if (occupiedContinents.ContainsKey(continentName))
            occupiedContinents[continentName] = false;
    }
}