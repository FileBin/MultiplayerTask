using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTask {
    public class HealthUI : MonoBehaviour {
        Image image;
        Vector3 initialScale;
        [SerializeField] Player player;
        void Start() {
            image = GetComponent<Image>();
            initialScale = image.rectTransform.localScale;
        }

        void Update() {
            var scale = initialScale;
            scale.x *= (float)player.Health / player.MaxHealth;
            image.rectTransform.localScale = scale;
        }
    }
}