using UnityEngine;

[CreateAssetMenu(fileName = "MejoraProteccion", menuName = "TiposDeMejora/MejoraProteccion")]
public class MejoraProteccion : MejoraData
{
    public int valor;

    public override void Aplicar(PlayerPCs player)
    {
        player.UpdateProteccion(PlayerPCs.ProtectionState.Baja);
    }
}
