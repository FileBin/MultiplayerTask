using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    void Start() {
        var args = System.Environment.GetCommandLineArgs();
        if (args.Contains("-game")) {
            Join();
        }
        NetworkManager.Singleton?.Shutdown();
    }
    public void Join() {
        SceneManager.LoadScene("GameMultiplayer");
    }

    public void PlaySingle() {
        SceneManager.LoadScene("GameSingle");
    }
}
