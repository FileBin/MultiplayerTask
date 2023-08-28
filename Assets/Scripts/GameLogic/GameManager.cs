using System;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MultiplayerTask {
    public class GameManager : NetworkBehaviour {
        public static GameManager Instance { get; private set; }

        enum GameStage { WaitingForPlayers, BattleStart, Battle, EndGame, Exit }
        GameStage stage = GameStage.WaitingForPlayers;

        int winnerIndex = -1;
        int winnerCoins = 0;
        [SerializeField] bool isSinglePlayer = false;
        [SerializeField] ObjectSpawner CoinSpawner;

        NetworkVariable<FixedString128Bytes> serverMessage = new NetworkVariable<FixedString128Bytes>("");

        public string ServerMessage { get => serverMessage.Value.ToString(); private set => serverMessage.Value = value; }
        public float countdownTimer = 3f;

        void Awake() {
            if (Instance != null) {
                throw new Exception("In scene can be only one GameManager!");
            }
            Instance = this;
        }

        public override void OnNetworkSpawn() {
            if (IsClient) {
                NetworkManager.Singleton.OnClientDisconnectCallback += id => {
                    LoadLobby();
                };
            }
        }

        void Update() {
            if (isSinglePlayer) {
                stage = GameStage.Battle;
                InitPlayers();
                return;
            }
            if (!IsServer) return;
            switch (stage) {
                case GameStage.WaitingForPlayers:
                    WaitingForPlayers();
                    break;
                case GameStage.BattleStart:
                    BattleStart();
                    break;
                case GameStage.Battle:
                    Battle();
                    break;
                case GameStage.EndGame:
                    EndGame();
                    break;
                case GameStage.Exit:
                    Exit();
                    break;
            }
        }

        private void WaitingForPlayers() {
            ServerMessage = "Waiting for players...";
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 2) {
                stage = GameStage.BattleStart;
                CoinSpawner.enabled = true;
            }
        }

        private void BattleStart() {
            int time = Mathf.CeilToInt(countdownTimer);

            ServerMessage = string.Format("{0}...", time);
            if (countdownTimer < 0) {
                InitPlayers();
                stage = GameStage.Battle;
            }

            countdownTimer -= Time.deltaTime;
        }

        private void Battle() {
            ServerMessage = "";
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length != 2) {
                SetWinner(1, players[0].GetComponent<Player>());
                return;
            }
            for (int i = 0; i < players.Length; i++) {
                var player = players[i].GetComponent<Player>();
                if (player.IsDead) {
                    var winner_index = (i + 1) % 2;
                    player.m_playable.Value = false;
                    SetWinner(winner_index + 1, players[winner_index].GetComponent<Player>());
                    return;
                }
            }
        }

        void SetWinner(int index, Player winner) {
            winnerIndex = index;
            countdownTimer = 3;
            stage = GameStage.EndGame;
            winnerCoins = winner.Coins;
        }

        private void EndGame() {
            ServerMessage = string.Format("Winner is Player{0} Coins: {1}", winnerIndex, winnerCoins);
            if (countdownTimer < 0) {
                stage = GameStage.Exit;
            }
            countdownTimer -= Time.deltaTime;
        }

        private void Exit() {
            if (NetworkManager.Singleton.ShutdownInProgress) return;
            NetworkManager.Singleton.Shutdown();
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(scene.name);
        }

        public void LoadLobby() {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Lobby");
        }

        private void InitPlayers() {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players) {
                var player = p.GetComponent<Player>();
                player.m_playable.Value = true;
            }
        }
    }
}