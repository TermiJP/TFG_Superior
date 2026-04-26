using UnityEngine;

[CreateAssetMenu(fileName = "MejoraPC", menuName = "TiposDeMejora/MejoraPC")]
public class MejoraPC : MejoraData
{
    public int valor;

    public override void Aplicar(PlayerPCs player)
    {
        player.cantidadPcsSinPoner += valor;
    }
}
