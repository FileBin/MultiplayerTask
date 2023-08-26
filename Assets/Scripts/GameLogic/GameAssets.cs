using UnityEngine;

namespace MultiplayerTask {
    [CreateAssetMenu(fileName = "GameAssets", menuName = "ScriptableObjects/CreateGameAssets")]
    public class GameAssets : ScriptableObject {
        private static GameAssets _instance;
        public static GameAssets Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<GameAssets>("GameAssets");
                }
                return _instance;
            }
        }

        [SerializeField] private GameObject textPrefab;
        public GameObject TextPrefab { get => textPrefab; }
    }
}