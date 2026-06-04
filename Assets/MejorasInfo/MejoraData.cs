using UnityEngine;
using static PlayerPCs;

[CreateAssetMenu(fileName = "MejoraData", menuName = "MEJORA")]
public abstract class MejoraData : ScriptableObject
{
    public Sprite sprite;
    public string NameMejora;
    public int Cost;
    public int valor;
    public TipoMejora tipo;
    public string Description;

    public abstract void Aplicar(PlayerPCs player);
    
    
}
