using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiplayerTask {
    public class ObjectSpawner : MonoBehaviour {
        [SerializeField] Vector2 area = Vector2.one * 10;
        [SerializeField] float spawnDistanceBetween = 1f;
        [SerializeField] float spawnTimer = 10f;
        [SerializeField] float timerRandomizer = 1f;
        [SerializeField] int spawnCount = 3;
        [SerializeField] int spawnCountRandomizer = 2;
        [SerializeField] int maxSpawnCount = 20;

        [SerializeField] GameObject gameObjectPrefab;

        float timer;
        private void Start() {
            GenNextTimer();
        }

        void GenNextTimer() {
            timer = spawnTimer + Random.Range(-timerRandomizer, timerRandomizer);
        }

        void FixedUpdate() {
            if (timer < 0) {
                SpawnObjects();
                GenNextTimer();
            }
            timer -= Time.fixedDeltaTime;
        }

        private void SpawnObjects() {
            if (transform.childCount > maxSpawnCount) return;
            var count = spawnCount + Random.Range(-spawnCountRandomizer, spawnCountRandomizer + 1);
            var obstacles = new LinkedList<Vector3>();

            foreach (Transform t in transform) {
                obstacles.AddLast(t.localPosition);
            }

            for (int i = 0; i < count; i++) {
                if (!GenPosition(obstacles, out var position)) return;
                SpawnObject(position);
                obstacles.AddLast(position);
            }
        }

        private void SpawnObject(Vector3 position) {
            var instance = Instantiate(gameObjectPrefab, transform);
            instance.transform.localPosition = position;
        }

        private bool GenPosition(IEnumerable<Vector3> obstacles, out Vector3 position) {
            const int maxIterations = 100;
            int i = 0;
            do {
                i++;
                position = new Vector3(Random.Range(-area.x, area.x), Random.Range(-area.y, area.y));
                if (i > maxIterations) return false;
            } while (!CheckDistance(position, obstacles));
            return true;
        }

        private bool CheckDistance(Vector3 position, IEnumerable<Vector3> obstacles) {
            foreach (Vector3 o in obstacles) {
                if (Vector3.Distance(o, position) < spawnDistanceBetween) {
                    return false;
                }
            }
            return true;
        }


        public void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Vector3[] points = new Vector3[] {
            new Vector3(area.x, area.y),
            new Vector3(-area.x, area.y),
            new Vector3(-area.x, -area.y),
            new Vector3(area.x, -area.y),
        };
            for (int i = 1; i < points.Length; i++) {
                Gizmos.DrawLine(transform.TransformPoint(points[i - 1]), transform.TransformPoint(points[i]));
            }
            Gizmos.DrawLine(transform.TransformPoint(points.Last()), transform.TransformPoint(points.First()));
        }
    }
}
