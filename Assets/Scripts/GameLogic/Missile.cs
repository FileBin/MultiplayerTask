using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace MultiplayerTask {
    public class Missile : NetworkBehaviour {
        public int Damage { get; set; }
        public Player player { get; set; }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!IsServer) return;
            if (other.isTrigger) return;
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
            ShowDamageClientRpc(Damage, transform.position);
        }

        [ClientRpc]
        private void ShowDamageClientRpc(int damage, Vector3 worldPos) {
            var text = Instantiate(
                GameAssets.Instance.TextPrefab, worldPos, Quaternion.identity);
            text.GetComponent<TextMeshPro>().text = damage.ToString();
        }
    }
}
