using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
namespace MultiplayerTask {
    public abstract class PlayerFinder : MonoBehaviour {
        enum UpdateType { Update, FixedUpdate, LateUpdate }
        [SerializeField] UpdateType updateType = UpdateType.Update;
        protected GameObject player { get; private set; }

        void Update() {
            if (updateType == UpdateType.Update)
                DefaultUpdate();
        }

        void FixedUpdate() {
            if (updateType == UpdateType.FixedUpdate)
                DefaultUpdate();
        }

        void LateUpdate() {
            if (updateType == UpdateType.LateUpdate)
                DefaultUpdate();
        }


        void DefaultUpdate() {
            if (player == null) {
                FindPlayer();
            }
            if (player != null) {
                UpdatePlayer();
            }
        }

        private void FindPlayer() {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players) {
                var o = p.GetComponent<NetworkObject>();
                if (o != null && o.IsLocalPlayer) {
                    player = p;
                    OnPlayerFound();
                    return;
                }
            }
        }

        public abstract void UpdatePlayer();
        public abstract void OnPlayerFound();
    }
}