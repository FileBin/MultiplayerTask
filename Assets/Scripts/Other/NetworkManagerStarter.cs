using Unity.Netcode;
using UnityEngine;
using System.Linq;

namespace MultiplayerTask {
    class NetworkManagerStarter : MonoBehaviour {
        [SerializeField] bool isSinglePlayer = false;
        public void Start() {
            NetworkManager.Singleton.Shutdown();
        }

        public void Update() {
            if (NetworkManager.Singleton.ShutdownInProgress) return;
            var args = System.Environment.GetCommandLineArgs().ToList();
            //args.Add("-server");
            if (args.Contains("-server")) {
                if (NetworkManager.Singleton.IsServer) return;
                NetworkManager.Singleton.StartServer();
            } else {
                if (isSinglePlayer) {
                    if (NetworkManager.Singleton.IsHost) return;
                    NetworkManager.Singleton.StartHost();
                } else {
                    if (NetworkManager.Singleton.IsClient) return;
                    NetworkManager.Singleton.StartClient();
                    enabled = false;
                }
            }
        }
    }
}