using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTask {
    public class HealthUI : PlayerFinder {
        Image image;
        Vector3 initialScale;
        Player playerScript;

        void Start() {
            image = GetComponent<Image>();
            initialScale = image.rectTransform.localScale;
        }

        public override void OnPlayerFound() {
            playerScript = player.GetComponent<Player>();
        }

        public override void UpdatePlayer() {
            var scale = initialScale;
            scale.x *= (float)playerScript.Health / playerScript.MaxHealth;
            image.rectTransform.localScale = scale;
        }
    }
}