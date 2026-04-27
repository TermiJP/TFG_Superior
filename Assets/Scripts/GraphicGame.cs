using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class GraphicGame : MonoBehaviour
{
    [Header("Control Sliders")]
    public Slider controlA;
    public Slider controlB;
    public Slider controlC;

    [Header("Column Sliders")]
    public Slider[] columns;

    [Header("Settings")]
    public float tolerance = 0.02f;

    [Header("Aviso")]
    public TMP_Text avisoWin;
    bool playerHasMoved;

    // Pesos: cada control afecta cada columna de forma distinta
    private readonly float[,] weights = new float[3, 4]
    {
        //  Col0    Col1    Col2    Col3
        {  2.0f,   0.5f,  -1.0f,   1.5f },   // Control A
        { -1.0f,   1.5f,   2.0f,  -0.5f },   // Control B
        {  0.5f,  -1.0f,   0.5f,   1.0f },   // Control C
    };

    // Offsets calculados matemáticamente para que la solución
    // sea exactamente A=0.65, B=0.40, C=0.75
    private readonly float[] baseOffsets = new float[4]
    {
        3.725f, 4.825f, 4.475f, 3.475f
    };

    // Rango real de valores posibles (calculado previamente)
    private const float MIN_RANGE = 2.7f;
    private const float MAX_RANGE = 7.0f;

    private float[] currentValues = new float[4];

    private void Start()
    {
        avisoWin.enabled = false;
    }

    void Update()
    {
        if (!IsValid()) return;
        ComputeValues();
        UpdateColumnSliders();

        if (!playerHasMoved)
        {
            if (controlA.value > 0.01f || controlB.value > 0.01f || controlC.value > 0.01f)
                playerHasMoved = true;
            else
                return;
        }

        
        CheckWin();
    }

    bool IsValid()
    {
        return controlA != null && controlB != null && controlC != null && columns != null;
    }

    void ComputeValues()
    {
        float A = controlA.value;
        float B = controlB.value;
        float C = controlC.value;

        for (int i = 0; i < columns.Length; i++)
        {
            currentValues[i] = baseOffsets[i]
                + weights[0, i] * A
                + weights[1, i] * B
                + weights[2, i] * C;
        }
    }

    void UpdateColumnSliders()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            if (columns[i] == null) continue;
            float normalized = Mathf.Clamp01(
                (currentValues[i] - MIN_RANGE) / (MAX_RANGE - MIN_RANGE)
            );
            columns[i].value = normalized;
        }
    }

    void CheckWin()
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < columns.Length; i++)
        {
            if (columns[i] == null) continue;
            float val = columns[i].value;
            Debug.Log($"Columna {i}: {val:F4}");  // ver valores reales
            if (val < min) min = val;
            if (val > max) max = val;
        }

        if (Mathf.Abs(max - min) < tolerance)
        {
            Debug.Log("SYSTEM STABILIZED - ACCESS GRANTED");
            //Codigo de que ha ganado le mini juego
            avisoWin.enabled = true;
            LockAll();
        }
    }

    void LockAll()
    {
        controlA.interactable = false;
        controlB.interactable = false;
        controlC.interactable = false;

        foreach (Slider col in columns)
        {
            if (col != null)
                col.interactable = false;
        }
    }


}
