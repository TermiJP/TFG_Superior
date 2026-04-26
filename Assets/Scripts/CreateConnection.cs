using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CreateConnection : NetworkBehaviour
{
    
    public List<Transform> Ordenadores = new List<Transform>();
    public int CantidadLines;
    public Transform origin;
    public Material connectionMat;
    public GameObject connectionPrefab;
    private GameObject newLine;
    private LineRenderer lineRenderer;

    void Start()
    {
        connectionPrefab.GetComponent<LineRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

    }



    public void AddObject(Transform newObject)
    {
        Ordenadores.Add(newObject);
        UpdateLine(newObject.transform.position);
        CantidadLines++;
    }

    void UpdateLine( Vector3 pc)
    {
        
        if( CantidadLines > 0 )
        {
            newLine = Instantiate(connectionPrefab);
            newLine.GetComponent<NetworkObject>().Spawn(true);
            lineRenderer = newLine.GetComponent<LineRenderer>();
            AsignTarget(lineRenderer, origin.position, pc);
        }
    }

    

    public void AsignTarget( LineRenderer line ,Vector3 startposition, Vector3 newTraget ) 
    {
        line.positionCount = 2;
        line.SetPosition(0, startposition);
        line.SetPosition(1,newTraget); 
    }
}
