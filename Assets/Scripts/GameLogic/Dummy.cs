using UnityEngine;

namespace MultiplayerTask {
    public class Dummy : MonoBehaviour, IDamagable {
        public int Health { get { return _health; } set { _health = value; if (_health < 0) _health = maxHealth; } }
        [SerializeField] int maxHealth = 50;
        int _health;
        public void Start() {
            _health = maxHealth;
        }
    }
}