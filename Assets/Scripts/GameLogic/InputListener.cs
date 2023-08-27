using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class InputListener : MonoBehaviour {
        private const string GameplayName = "gameplay";
        [SerializeField] InputActionAsset actions;

        protected InputActionMap ActionMap { get; private set; }
        void OnEnable() {
            ActionMap = actions.FindActionMap(GameplayName, true);
            ActionMap.Enable();
        }

        void OnDisable() {
            ActionMap.Disable();
        }
    }
}