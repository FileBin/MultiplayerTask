using System.Linq;
using TMPro;
using UnityEngine;

namespace MultiplayerTask {
    public class Missile : MonoBehaviour {
        public int Damage { get; set; }
        public Player player { get; set; }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.isTrigger) return;
            IDamagable other_player = other.GetComponents<MonoBehaviour>().OfType<IDamagable>().FirstOrDefault();
            if (other_player != (IDamagable)player) {
                if (other_player != null) {
                    Attack(other_player);
                }
                Destroy(gameObject);
            }
        }

        private void Attack(IDamagable damagable) {
            damagable.Health -= Damage;
            var text = Instantiate(
                GameAssets.Instance.TextPrefab, transform.position, Quaternion.identity);
            text.GetComponent<TextMeshPro>().text = Damage.ToString();
        }
    }
}
