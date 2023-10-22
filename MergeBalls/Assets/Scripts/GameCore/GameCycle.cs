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
        private CollisionSystem collesionSystem;
        private bool alreadyWin;
        private bool alreadyStarted = true;

        private List<GameActor> presettedActors;

        public event Action OnWin;

        public bool CanShoot => alreadyStarted && !alreadyWin && player != null;

        public GameCycle(SpawnManager spawnManager, IPointerEventHadler pointerUpHandler, GameRules rules, ActorsAggregator actorsAggregator) {
            this.actorsAggregator = actorsAggregator;
            this.rules = rules;
            this.spawnManager = spawnManager;
            pointerUpHandler.OnPointerEvent += OnTapComplete;
            playerSpawnPosition = SpawnHelpers.GetPlayerSpawnPosition();

            collesionSystem = new CollisionSystem(actorsAggregator, rules);
            collesionSystem.OnMerge += OnMerge;
            collesionSystem.OnCollide += OnCollide;
        }

        public async Task PrepareField() {
            player = await SpawnPlayer();
            presettedActors = spawnManager.SpawnPresettedActors();
            await Task.Delay(200);
        }

        public async Task Start() {
            alreadyStarted = true;
            await SpawnPresettedActors(presettedActors);
        }

        private async Task SpawnPresettedActors(List<GameActor> actors) {
            for (var i = 0; i < actors.Count; i++) {
                var actor = actors[i];
                PrepareActor(actor);
                await actor.AnimateSpawn();

                actor.SwitchGravity(true);
                await Task.Delay(500);
            }
        }

        private async Task<GameActor> SpawnPlayer() {
            var actor = spawnManager.SpawnPlayer();
            PrepareActor(actor);
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
            }

            var actor = spawnManager.SpawnOnMerge(position, grade);
            PrepareActor(actor);
            actor.SwitchGravity(true);
            await actor.AnimateSpawn();
        }

        private void OnCollide(Vector3 position, Color color1, Color color2, Vector2 sizes) {
            spawnManager.SpwanCollideEffect(position, color1, color2, sizes);
        }

        private void PrepareActor(GameActor actor) {
            actor.gameObject.SetActive(true);
            actor.OnCollision += collesionSystem.OnCollision;
            actor.OnTrigger += collesionSystem.OnTrigger;
            actorsAggregator.AddComponent(actor.ColliderId, actor);
            if (rules.UseSoftPrefabs) actorsAggregator.AddSoftCollider(actor.SoftCollider);
        }
    }
}
