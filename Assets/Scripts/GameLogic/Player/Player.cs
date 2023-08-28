using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class Player : InputListener, IDamagable {
        public int Coins { get => m_coins.Value; set => m_coins.Value = value; }
        public int Health {
            get => m_health.Value;
            set {
                if (value <= 0) {
                    OnDeath();
                    m_health.Value = 0;
                    return;
                }
                m_health.Value = value;

            }
        }

        public string PlayerName { get => m_playerName.Value.ToString(); }

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
        private NetworkVariable<bool> m_dead = new NetworkVariable<bool>();
        NetworkVariable<FixedString64Bytes> m_playerName = new NetworkVariable<FixedString64Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
        InputAction fireAutoAimAction;
        float recharge = 0f;
        bool flipX = false;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (!IsClient) return;
            if (!IsOwner) return;
            var id = NetworkManager.LocalClientId;
            if (id % 2 == 0) {
                transform.position = GameObject.FindGameObjectWithTag("Player1Spawn").transform.position;
                m_playerName.Value = "Player1";
            } else {
                transform.position = GameObject.FindGameObjectWithTag("Player2Spawn").transform.position;
                m_playerName.Value = "Player2";
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
                fireAutoAimAction = ActionMap.FindAction("autoaim", true);
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
            bool fire = rightStick.sqrMagnitude > 0.25f;
            if (fire) {
                LookDirection = rightStick.normalized;
                movement = rightStick;
            } else {
                var autoAim = fireAutoAimAction.ReadValue<float>() > 0.5f;
                if (autoAim) {
                    fire = true;
                    if (GetAimDirection(out var aimDirection)) {
                        LookDirection = aimDirection;
                        movement = aimDirection;
                    }
                }
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

        private bool GetAimDirection(out Vector2 aimDirection) {
            aimDirection = Vector2.left;
            var enemies = GameObject.FindGameObjectsWithTag("Player").Where(x => x != gameObject);
            var enumerator = enemies.GetEnumerator();
            if (!enumerator.MoveNext()) return false;
            aimDirection = enumerator.Current.transform.position - transform.position;
            while (enumerator.MoveNext()) {
                var nextDirection = enumerator.Current.transform.position - transform.position;
                if (nextDirection.sqrMagnitude < aimDirection.sqrMagnitude)
                    aimDirection = nextDirection;
            }
            aimDirection.Normalize();
            return true;
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
