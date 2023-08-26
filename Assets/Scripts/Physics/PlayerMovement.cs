using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class PlayerMovement : InputListener {
        [SerializeField, Range(1, 10)] float playerSpeed;
        [SerializeField] SpriteRenderer[] spriteRenderers;
        [SerializeField] Animator animator;
        [SerializeField] Player player;

        new Rigidbody2D rigidbody;
        InputAction moveAction;

        Vector2 movement;
        bool flipX = false;

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            moveAction = ActionMap.FindAction("move", true);

            if (animator == null) animator = GetComponent<Animator>();
            if (player == null) player = GetComponent<Player>();
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

            animator.SetFloat("speed", movement.magnitude);
        }

        void FixedUpdate() {
            rigidbody.SetRotation(Quaternion.identity);
            rigidbody.velocity = movement * playerSpeed;
        }
    }
}
