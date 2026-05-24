using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsWorld : MonoBehaviour
{
    [Header("Pilares (Image tipo Filled)")]
    public Image barYo;
    public Image barRival;
    public Image barCura;

    [Header("Textos de porcentaje")]
    public TMP_Text textoYo;
    public TMP_Text textoRival;
    public TMP_Text textoCura;

    private const long POBLACION_MUNDIAL = 8_000_000_000L;

    // Llama este método cada vez que cambien los datos
    public void ActualizarGrafica(long infectadosYo, long infectadosRival, float porcentajeCura)
    {
        float pctYo = (float)infectadosYo / POBLACION_MUNDIAL;
        float pctRival = (float)infectadosRival / POBLACION_MUNDIAL;
        float pctCura = Mathf.Clamp01(porcentajeCura / 100f);

        // fillAmount va de 0 a 1
        barYo.fillAmount = pctYo;
        barRival.fillAmount = pctRival;
        barCura.fillAmount = pctCura;

        textoYo.text = $"{pctYo * 100f:F1}%";
        textoRival.text = $"{pctRival * 100f:F1}%";
        textoCura.text = $"{porcentajeCura:F1}%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
