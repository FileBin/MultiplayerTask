using TMPro;
using UnityEngine;

namespace MultiplayerTask {
    public class PlayerNameDisplay : MonoBehaviour {
        [SerializeField] TextMeshPro text;
        [SerializeField] Player playerScript;

        void Update() {
            text.text = playerScript.PlayerName;
        }
    }
}