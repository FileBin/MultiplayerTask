using Unity.Netcode;
using UnityEngine;
using System.Linq;

namespace MultiplayerTask {
    class NetworkManagerStarter : MonoBehaviour {
        public void Start() {
#if !UNITY_EDITOR
            var args = System.Environment.GetCommandLineArgs();
            if(args.Contains("-server")) {
                NetworkManager.Singleton.StartServer();
            } else {
                NetworkManager.Singleton.StartClient();
            }
#endif
            print("NetworkManager started");
        }
    }
}