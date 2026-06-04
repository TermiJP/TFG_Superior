using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Desktop_UI : NetworkBehaviour
{
    [SerializeField] public Canvas canvas;
    public bool abierto;

    [Header("Pilares")]
    public Image barYo;
    public Image barRival;
    public Image barCura;

    [Header("Porcentaje")]
    public TMP_Text textoYo;
    public TMP_Text textoRival;
    public TMP_Text textoCura;

    private Coroutine _actualizacionLoop;
    private Coroutine _animacionLoop;

    [Header("Datos")]
    public long infectadosYo;
    public long infectadosRival;
    public float porcentajeCura;

    [Header("Configuracion")]
    [Range(0.1f, 2f)]
    public float intervaloActualizacion = 0.5f;
    [Range(1f, 10f)]
    public float velocidadAnimacion = 5f;

    private const long POBLACION_MUNDIAL = 8_000_000_000L;

    private float _targetYo;
    private float _targetRival;
    private float _targetCura;

    private CureSystem _curesystem;
    private PlayerPCs _playerLocal;
    private PlayerPCs _playerRival;

    [SerializeField] private Button figthButton;


    private void Start()
    {
        canvas.enabled = false;
        _curesystem = GameObject.Find("CureSystem").GetComponent<CureSystem>();
        figthButton.onClick.AddListener(FightPVP);
        FindPlayers();
    }

    private void Update()
    {
        porcentajeCura = _curesystem.cureProgress;

        if (_playerLocal == null || _playerRival == null)
            FindPlayers();

        if (_playerLocal != null)
            infectadosYo = _playerLocal.Peopleinfected.Value;

        if (_playerRival != null)
            infectadosRival = _playerRival.Peopleinfected.Value;

        if( infectadosRival >= POBLACION_MUNDIAL || infectadosYo >= POBLACION_MUNDIAL)
        {
            //AQUI SE TERMINA LA PARTIDA
        }
    }

    private void FindPlayers()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject go in allPlayers)
        {
            PlayerPCs p = go.GetComponent<PlayerPCs>();
            if (p == null) continue;

            if (p.IsOwner)
                _playerLocal = p;
            else
                _playerRival = p;
        }
    }

    
    public void AbrirDesktopRpc()
    {
        if (abierto == false)
        {
            canvas.enabled = true;
            abierto = true;
            _animacionLoop = StartCoroutine(AnimarBarras());
            _actualizacionLoop = StartCoroutine(ActualizarMientrasAbierto());
        } else
        {
            canvas.enabled = false;
            abierto = false;
            if (_actualizacionLoop != null) StopCoroutine(_actualizacionLoop);
            if (_animacionLoop != null) StopCoroutine(_animacionLoop);
        }
    }

    IEnumerator ActualizarMientrasAbierto()
    {
        while (true)
        {
            _targetYo = (float)infectadosYo / POBLACION_MUNDIAL;
            _targetRival = (float)infectadosRival / POBLACION_MUNDIAL;
            _targetCura = Mathf.Clamp01(porcentajeCura / 100f);

            textoYo.text = $"{_targetYo * 100f:F1}%";
            textoRival.text = $"{_targetRival * 100f:F1}%";
            textoCura.text = $"{porcentajeCura:F1}%";

            yield return new WaitForSeconds(intervaloActualizacion);
        }
    }

    IEnumerator AnimarBarras()
    {
        float t = 0f;
        float fromYo = barYo.fillAmount;
        float fromRival = barRival.fillAmount;
        float fromCura = barCura.fillAmount;

        while (t < 1f)
        {
            barYo.fillAmount = Mathf.Lerp(barYo.fillAmount, _targetYo, Time.deltaTime * velocidadAnimacion);
            barRival.fillAmount = Mathf.Lerp(barRival.fillAmount, _targetRival, Time.deltaTime * velocidadAnimacion);
            barCura.fillAmount = Mathf.Lerp(barCura.fillAmount, _targetCura, Time.deltaTime * velocidadAnimacion);

            yield return null;
        }
    }
    void FightPVP()
    {
        if (_playerLocal.xpOrdenaodres.Value < 25)
        {
            Debug.Log("No tienes XP suficiente");
            return;
        }

        GastarXPServerRpc(25);
        _playerLocal.StartMinigameRpc();
    }

    [Rpc(SendTo.Server)]
    public void GastarXPServerRpc(float cantidad)
    {
        if (_playerLocal.xpOrdenaodres.Value >= cantidad)
            _playerLocal.xpOrdenaodres.Value -= cantidad;
    }
}
