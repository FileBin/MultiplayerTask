using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class InputListener : NetworkBehaviour {
        private const string GameplayName = "gameplay";
        [SerializeField] InputActionAsset actions;

        protected InputActionMap ActionMap { get; private set; }

        public override void OnNetworkSpawn() {
            if(!IsClient) return;
            ActionMap = actions.FindActionMap(GameplayName, true);
            ActionMap.Enable();
        }

        public override void OnNetworkDespawn() {
            if(!IsClient) return;
            ActionMap.Disable();
        }
    }
}