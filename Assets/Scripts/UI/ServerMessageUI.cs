using TMPro;
using UnityEngine;

namespace MultiplayerTask {
    public class ServerMessageUI : MonoBehaviour {
        TextMeshProUGUI textUI;
        void Start() {
            textUI = GetComponent<TextMeshProUGUI>();
        }

        void Update() {
            textUI.text = GameManager.Instance.ServerMessage;
        }
    }
}