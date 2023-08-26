using UnityEngine;

namespace MultiplayerTask {
    public class Coin : MonoBehaviour {
        public bool IsCollectable { get; private set; }

        private void Start() {
            IsCollectable = true;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var player = other.GetComponent<Player>();
            if (player != null) {
                Collect(player);
            }
        }

        private void Collect(Player player) {
            if (IsCollectable) {
                IsCollectable = false;
                player.Coins++;
                Destroy(gameObject);
            }
        }
    }
}
