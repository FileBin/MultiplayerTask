using UnityEngine;
using System.Linq;

namespace MultiplayerTask {
    public class FrameCamera : MonoBehaviour {
        [SerializeField] Vector2 frameSize = Vector2.one;
        [SerializeField, Range(0, 1)] float interpolationSpeed = 0.8f;
        [SerializeField] GameObject player;

        Vector3 targetCameraPosition;
        float depthPosition;

        void Start() {
            depthPosition = transform.position.z;
            targetCameraPosition = player.transform.position;
            UpdatePosition();
        }

        void FixedUpdate() {
            CalculateTargetPosition();
            UpdatePosition();
        }

        void CalculateTargetPosition() {
            var playerRelative = player.transform.position - targetCameraPosition;

            for (int i = 0; i < 2; i++) {
                playerRelative[i] = Mathf.Sign(playerRelative[i]) * Mathf.Max(Mathf.Abs(playerRelative[i]) - frameSize[i], 0);
            }
            targetCameraPosition += playerRelative;
        }

        void UpdatePosition() {
            var target = targetCameraPosition + Vector3.forward * depthPosition;
            transform.position = Vector3.Lerp(transform.position, target, interpolationSpeed);
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Vector3[] points = new Vector3[] {
            new Vector3(frameSize.x, frameSize.y),
            new Vector3(-frameSize.x, frameSize.y),
            new Vector3(-frameSize.x, -frameSize.y),
            new Vector3(frameSize.x, -frameSize.y),
        };
            for (int i = 1; i < points.Length; i++) {
                Gizmos.DrawLine(transform.TransformPoint(points[i - 1]), transform.TransformPoint(points[i]));
            }
            Gizmos.DrawLine(transform.TransformPoint(points.Last()), transform.TransformPoint(points.First()));
        }
    }
}