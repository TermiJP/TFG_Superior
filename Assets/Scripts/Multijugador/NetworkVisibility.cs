using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Attach this to the PC prefab.
/// Controls visibility per-client via a NetworkVariable<bool>.
/// The object is spawned invisible and becomes visible when SetVisible(true) is called.
/// </summary>
public class NetworkVisibility : NetworkBehaviour
{
    // ── NetworkVariable ──────────────────────────────────────────────────────
    // Only the server/host can write; all clients can read and react.
    private NetworkVariable<ulong> _ownerClientId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // ── Cached references ─────────────────────────────────────────────────
    // Adjust if your renderers are on child objects – GetComponentsInChildren
    // grabs everything in the hierarchy.
    private Renderer[] _renderers;

    // ── Unity lifecycle ───────────────────────────────────────────────────
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    public override void OnNetworkSpawn()
    {
        // El dueño siempre ve su propio objeto
        if (IsOwner)
        {
            ApplyVisibility(true, Color.aliceBlue);
        }
        else
        {
            // El contrario empieza sin verlo
            ApplyVisibility(true, Color.orangeRed);
        }
    }

    public void InitOwner(ulong placerClientId)
    {
        if (!IsServer) return;
        _ownerClientId.Value = placerClientId;
    }

   

    // ── Public API ────────────────────────────────────────────────────────

    public void SetVisibleForOthers(bool visible)
    {
        if (!IsServer)
        {
            Debug.LogWarning("[NetworkVisibility] Solo el servidor puede llamar SetVisibleForOthers.");
            return;
        }

        ulong owner = _ownerClientId.Value;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId == owner) continue; // El dueño nunca se toca

            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            SetVisibilityClientRpc(visible, rpcParams);
        }
    }

    // ── ClientRpc dirigido solo al cliente contrario ─────────────────────────
    [ClientRpc]
    private void SetVisibilityClientRpc(bool visible, ClientRpcParams rpcParams = default)
    {
        // Doble check: el dueño nunca obedece este RPC aunque llegara por error
        if (IsOwner) return;

        ApplyVisibility(visible,Color.red);
    }

    public void ApplyVisibility(bool visible , Color color )
    {
        foreach (var r in _renderers)
        {
            r.enabled = visible;
            r.material.color = color;
        }
            
    }
}
