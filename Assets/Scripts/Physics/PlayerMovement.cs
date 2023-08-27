using System;
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
            moveAction = ActionMap.FindAction("move", true);

            if (animator == null) animator = GetComponent<Animator>();
            if (player == null) player = GetComponent<Player>();
        }

        void Update() {
            movement = moveAction.ReadValue<Vector2>();
            animator.SetFloat("speed", rigidbody.velocity.magnitude);
        }

        void FixedUpdate() {
            rigidbody.SetRotation(Quaternion.identity);
            rigidbody.velocity = movement * playerSpeed;
        }
    }
}
