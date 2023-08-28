using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class Player : InputListener, IDamagable {
        public int Coins { get => m_coins.Value; set => m_coins.Value = value; }
        public int Health {
            get => m_health.Value;
            set {
                m_health.Value = value;
                if (m_health.Value < 0) {
                    OnDeath();
                }
            }
        }

        public bool IsPlayable { get => m_playable.Value; }
        public bool IsDead { get => m_dead.Value; }

        private void OnDeath() {
            if (!IsServer) return;
            m_dead.Value = true;
        }

        public int MaxHealth { get => maxHealth; }
        public Vector2 LookDirection { get; private set; }

        private NetworkVariable<int> m_coins = new NetworkVariable<int>();
        private NetworkVariable<int> m_health = new NetworkVariable<int>();
        internal NetworkVariable<bool> m_playable = new NetworkVariable<bool>();
        internal NetworkVariable<bool> m_dead = new NetworkVariable<bool>();

        [SerializeField] int maxHealth = 20;
        [SerializeField] float rechargeTime = 20;
        [SerializeField] float kritRate = 0.25f;
        [SerializeField] int kritDamage = 2;
        [SerializeField] float missileSpeed = 15f;

        [SerializeField] SpriteRenderer missileIdicator;
        [SerializeField] GameObject missilePrefab;
        [SerializeField] Transform LookDirectionIndicator;
        [SerializeField] SpriteRenderer[] spriteRenderers;

        new Rigidbody2D rigidbody;
        InputAction fireDirectionAction;
        float recharge = 0f;
        bool flipX = false;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (!IsClient) return;
            if (!IsOwner) return;
            var id = NetworkManager.LocalClientId;
            if (id % 2 == 0) {
                transform.position = GameObject.FindGameObjectWithTag("Player1Spawn").transform.position;
            } else {
                transform.position = GameObject.FindGameObjectWithTag("Player2Spawn").transform.position;
            }
        }

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            Random.InitState(System.DateTime.Now.Millisecond);
            if (IsServer) {
                Health = maxHealth;
            }
            LookDirection = Vector2.left;
            if (IsClient) {
                fireDirectionAction = ActionMap.FindAction("fire", true);
            }
        }

        void Update() {
            RotateIndicator();
            UpdateInput();
        }

        void UpdateInput() {
            if (!IsClient) return;
            if (!IsOwner) return;
            if (!IsPlayable) return;
            bool canFire = recharge <= 0f;
            missileIdicator.enabled = canFire;
            var movement = rigidbody.velocity;

            var rightStick = fireDirectionAction.ReadValue<Vector2>();
            bool fire = rightStick.sqrMagnitude > 0.1f;
            if (fire) {
                LookDirection = rightStick.normalized;
                movement = rightStick;
            }

            if (movement.x > Vector2.kEpsilon)
                flipX = true;
            else if (movement.x < -Vector2.kEpsilon) {
                flipX = false;
            }

            for (int i = 0; i < spriteRenderers.Length; i++) {
                spriteRenderers[i].flipX = flipX;
            }

            if (fire && canFire) {
                recharge = rechargeTime;
                LaunchMissileServerRpc(LookDirection, GetLookRotation());
            } else {
                recharge -= Time.deltaTime;
            }
        }

        [ServerRpc]
        void LaunchMissileServerRpc(Vector3 direction, Quaternion rotation) {
            var missileObject = Instantiate(missilePrefab);
            var missileScript = missileObject.GetComponent<Missile>();
            var missileRigidbody = missileObject.GetComponent<Rigidbody2D>();

            missileScript.player = this;
            missileScript.Damage = 1;

            if (Random.Range(0f, 1f) < kritRate) {
                missileScript.Damage += kritDamage;
            }

            missileObject.transform.rotation = rotation;
            missileObject.transform.position = transform.position + new Vector3(direction.x, direction.y, 0);
            missileRigidbody.velocity = direction.normalized * missileSpeed;
            missileObject.GetComponent<NetworkObject>().Spawn(true);
        }

        void RotateIndicator() {
            if (!IsOwner) return;
            if (LookDirectionIndicator != null) {
                LookDirectionIndicator.rotation = GetLookRotation();
            }
        }

        Quaternion GetLookRotation() => Quaternion.Euler(0, 0, Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg);
    }
}
