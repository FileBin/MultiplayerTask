using UnityEngine;

namespace MultiplayerTask {
    public class Player : MonoBehaviour {
        public int Coins { get; set; }
        public int Health { get; set; }
        public Vector2 LookDirection { get; set; }

        [SerializeField] int maxHealth = 20;
        [SerializeField] float kritRate = 0.25f;
        [SerializeField] float kritDamage = 2f;

        [SerializeField] Transform LookDirectionIndicator;

        void Start() {
            Health = maxHealth;
        }

        void Update() {
            if (LookDirectionIndicator != null) {
                float rotationZ = Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg;
                LookDirectionIndicator.rotation = Quaternion.Euler(0, 0, rotationZ);
            }
        }
    }
}
