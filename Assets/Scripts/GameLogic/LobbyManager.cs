using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    void Start() {
        var args = System.Environment.GetCommandLineArgs();
        if (args.Contains("-game")) {
            Join();
        }
    }
    public void Join() {
        SceneManager.LoadScene("GameMultiplayer");
    }
}
