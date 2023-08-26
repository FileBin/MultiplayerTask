using UnityEngine;

namespace MultiplayerTask {
    public class Missile : MonoBehaviour {
        public int Damage { get; set; }
        public Player player { get; set; }

        private void OnTriggerEnter2D(Collider2D other) {
            var other_player = other.GetComponent<Player>();
            if (other_player != player) {
                if (other_player != null) {
                    Attack(other_player);
                }
                Destroy(gameObject);
            }
        }

        private void Attack(Player player) {
            player.Health -= Damage;
        }
    }
}
