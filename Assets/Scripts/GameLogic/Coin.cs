using Unity.Netcode;
using UnityEngine;

namespace MultiplayerTask {
    public class Coin : NetworkBehaviour {
        public bool IsCollectable { get; private set; }

        private void Start() {
            IsCollectable = true;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!IsServer) return;
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
