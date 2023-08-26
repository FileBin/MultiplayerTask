using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class PlayerMovement : MonoBehaviour {
        private const string GameplayName = "gameplay";

        [SerializeField, Range(1, 10)] float playerSpeed;
        [SerializeField] InputActionAsset actions;
        [SerializeField] SpriteRenderer[] spriteRenderers;
        [SerializeField] Animator animator;
        [SerializeField] Player player;

        new Rigidbody2D rigidbody;
        InputAction moveAction;

        Vector2 movement;
        bool flipX = false;

        void OnEnable() {
            actions.FindActionMap(GameplayName).Enable();
        }

        void OnDisable() {
            actions.FindActionMap(GameplayName).Disable();
        }

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            actions.Enable();
            moveAction = actions.FindActionMap(GameplayName, true).FindAction("move", true);

            if (animator == null) animator = GetComponent<Animator>();
            if (player == null) player = GetComponent<Player>();

            player.LookDirection = Vector2.left;
        }

        void Update() {
            movement = moveAction.ReadValue<Vector2>();
            if (movement.x > Vector2.kEpsilon)
                flipX = true;
            else if (movement.x < -Vector2.kEpsilon) {
                flipX = false;
            }
            for (int i = 0; i < spriteRenderers.Length; i++) {
                spriteRenderers[i].flipX = flipX;
            }

            if (movement.sqrMagnitude > Vector2.kEpsilon) {
                player.LookDirection = movement.normalized;
            }

            animator.SetFloat("speed", movement.magnitude);
        }

        void FixedUpdate() {
            rigidbody.SetRotation(Quaternion.identity);
            rigidbody.velocity = movement * playerSpeed;
        }
    }
}
