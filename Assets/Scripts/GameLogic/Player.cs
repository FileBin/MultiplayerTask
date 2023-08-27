using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class Player : InputListener, IDamagable {
        public int Coins { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get => maxHealth; }
        public Vector2 LookDirection { get; private set; }

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

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            Random.InitState(System.DateTime.Now.Millisecond);
            Health = maxHealth;
            fireDirectionAction = ActionMap.FindAction("fire", true);
            LookDirection = Vector2.left;
        }

        void Update() {
            RotateIndicator();
            UpdateInput();
        }

        private void UpdateInput() {
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
                LaunchMissile();
            } else {
                recharge -= Time.deltaTime;
            }
        }

        private void LaunchMissile() {
            var missileObject = Instantiate(missilePrefab);
            var missileScript = missileObject.GetComponent<Missile>();
            var missileRigidbody = missileObject.GetComponent<Rigidbody2D>();

            missileScript.player = this;
            missileScript.Damage = 1;
            if (Random.Range(0f, 1f) < kritRate) {
                missileScript.Damage += kritDamage;
            }
            missileObject.transform.rotation = GetLookRotation();
            missileObject.transform.position = transform.position + new Vector3(LookDirection.x, LookDirection.y, 0);
            missileRigidbody.velocity = LookDirection * missileSpeed;
        }

        void RotateIndicator() {
            if (LookDirectionIndicator != null) {
                LookDirectionIndicator.rotation = GetLookRotation();
            }
        }

        Quaternion GetLookRotation() => Quaternion.Euler(0, 0, Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg);
    }
}
