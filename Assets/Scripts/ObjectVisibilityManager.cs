using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectVisibilityManager : NetworkBehaviour
{
    private List<NetworkObject> _Ordenadores;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNetworkObject( NetworkObject prefab )
    {
        _Ordenadores.Add(prefab);
    }
}
