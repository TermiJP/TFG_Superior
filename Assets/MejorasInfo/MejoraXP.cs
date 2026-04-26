using UnityEngine;

[CreateAssetMenu(fileName = "MejoraXP", menuName = "TiposDeMejora/MejoraXP")]
public class MejoraXP : MejoraData
{
    public int valor;

    public override void Aplicar(PlayerPCs player)
    {
        player._sumaxp += valor;
    }
}
