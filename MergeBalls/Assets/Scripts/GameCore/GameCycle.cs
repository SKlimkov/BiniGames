using BiniGames.GameCore.Spawn;
using BiniGames.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BiniGames.GameCore {
    public class GameCycle {
        private SpawnManager spawnManager;
        private GameActor player;
        private GameRules rules;
        private Vector3 playerSpawnPosition;
        private ActorsAggregator actorsAggregator;
        private MergeSystem mergeSystem;
        private bool alreadyWin;

        private List<GameActor> presettedActors;

        public event Action OnWin;

        public bool CanShoot => !alreadyWin && player != null;

        public GameCycle(SpawnManager spawnManager, IPointerEventHadler pointerUpHandler, GameRules rules, ActorsAggregator actorsAggregator) {
            this.actorsAggregator = actorsAggregator;
            mergeSystem = new MergeSystem(actorsAggregator);
            mergeSystem.OnMerge += OnMerge;
            this.rules = rules;
            this.spawnManager = spawnManager;
            pointerUpHandler.OnPointerEvent += OnTapComplete;
            playerSpawnPosition = SpawnHelpers.GetPlayerSpawnPosition();
        }

        public async Task PrepareField() {
            player = await SpawnPlayer();
            presettedActors = spawnManager.SpawnPresettedActors();
            await Task.Delay(200);
        }

        public async Task Start() {
            await SpawnPresettedActors(presettedActors);
        }

        private async Task SpawnPresettedActors(List<GameActor> actors) {
            for (var i = 0; i < actors.Count; i++) {
                var actor = actors[i];
                actor.gameObject.SetActive(true);
                actor.OnCollide += mergeSystem.OnCollision;
                actorsAggregator.AddActor(actor);
                await actor.AnimateSpawn();

                actor.SwitchGravity(true);
                await Task.Delay(500);
            }
        }

        private async Task<GameActor> SpawnPlayer() {
            var actor = spawnManager.SpawnPlayer();
            actor.gameObject.SetActive(true);
            actor.OnCollide += mergeSystem.OnCollision;
            actor.SwitchGravity(false);
            actorsAggregator.AddActor(actor);
            await actor.AnimateSpawn();

            return actor;
        }

        private async void OnTapComplete(Vector2 targetPosition) {
            if (!CanShoot) return;

            player.SwitchGravity(true);
            var direction = Camera.main.ScreenToWorldPoint(targetPosition) - playerSpawnPosition;
            player.Fire(direction.normalized * rules.ForcePower);
            player = null;
            await Task.Delay(300);
            player = await SpawnPlayer();
        }

        private async void OnMerge(Vector3 position, int grade) {
            if (!alreadyWin && grade >= rules.WinGrade) {
                alreadyWin = true;
                OnWin?.Invoke();
                return;
            }

            var actor = spawnManager.SpawnOnMerge(position, grade);
            actor.gameObject.SetActive(true);
            actor.OnCollide += mergeSystem.OnCollision;
            actorsAggregator.AddActor(actor);
            actor.SwitchGravity(true);
            await actor.AnimateSpawn();
        }
    }
}
