using UnityEngine;

[CreateAssetMenu(fileName = "MejoraPeligro", menuName = "TiposDeMejora/MejoraPeligro")]
public class MejoraPeligro : MejoraData
{
    public int valor;

    public override void Aplicar(PlayerPCs player)
    {
        player.peligroHacker += valor;
    }
}
