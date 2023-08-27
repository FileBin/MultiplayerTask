using TMPro;
using UnityEngine;

namespace MultiplayerTask {
    public class CoinsUI : PlayerFinder {
        TextMeshProUGUI textUI;
        string textFormat;
        Player playerScript;
        void Start() {
            textUI = GetComponent<TextMeshProUGUI>();
            textFormat = textUI.text;
        }

        public override void OnPlayerFound() {
            playerScript = player.GetComponent<Player>();
        }

        public override void UpdatePlayer() {
            textUI.text = string.Format(textFormat, playerScript.Coins);
        }
    }
}