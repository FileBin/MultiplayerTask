using TMPro;
using UnityEngine;

namespace MultiplayerTask {
    public class CoinsUI : MonoBehaviour {
        TextMeshProUGUI textUI;
        string textFormat;
        [SerializeField] Player player;
        void Start() {
            textUI = GetComponent<TextMeshProUGUI>();
            textFormat = textUI.text;
        }

        void Update() {
            textUI.text = string.Format(textFormat, player.Coins);
        }
    }
}