using UnityEngine;

namespace MultiplayerTask {
    public class DestroyTimer : MonoBehaviour {
        [SerializeField] float timer = 1;

        void FixedUpdate() {
            timer -= Time.fixedDeltaTime;
            if(timer <= 0) {
                Destroy(gameObject);
            }
        }
    }
}