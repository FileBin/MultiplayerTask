using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NetworkSprite : NetworkBehaviour {
    NetworkVariable<bool> n_flipX = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> n_flipY = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> n_enabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            n_flipX.OnValueChanged += (bool old, bool val) => { spriteRenderer.flipX = val; };
            n_flipY.OnValueChanged += (bool old, bool val) => { spriteRenderer.flipY = val; };
            n_enabled.OnValueChanged += (bool old, bool val) => { spriteRenderer.enabled = val; };
            return;
        }
        n_flipX.Value = spriteRenderer.flipX;
        n_flipY.Value = spriteRenderer.flipY;
        n_enabled.Value = spriteRenderer.enabled;
    }

    void Update() {
        if (!IsOwner) return;
        n_flipX.Value = spriteRenderer.flipX;
        n_flipY.Value = spriteRenderer.flipY;
        n_enabled.Value = spriteRenderer.enabled;
    }
}
