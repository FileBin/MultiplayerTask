using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTask {
    public class PlayerMovement : InputListener {
        [SerializeField, Range(1, 10)] float playerSpeed;
        [SerializeField] Animator animator;
        [SerializeField] Player player;

        new Rigidbody2D rigidbody;
        InputAction moveAction;

        Vector2 movement;

        void Start() {
            rigidbody = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (player == null) player = GetComponent<Player>();
            if (IsClient) {
                moveAction = ActionMap.FindAction("move", true);
            }
        }

        void Update() {
            if (!IsOwner) return;
            var animatorSpeed = rigidbody.velocity.magnitude;
            animator.SetFloat("speed", animatorSpeed);
            AnimateServerRpc(animatorSpeed);
            if (!player.IsPlayable) return;
            movement = moveAction.ReadValue<Vector2>();
        }

        [ServerRpc]
        void AnimateServerRpc(float speed) {
            animator.SetFloat("speed", speed);
        }

        void FixedUpdate() {
            rigidbody.SetRotation(Quaternion.identity);

            rigidbody.velocity = movement * playerSpeed;
        }
    }
}
