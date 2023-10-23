using BiniGames.GameCore.Spawn;
using BiniGames.Input;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace BiniGames.GameCore {
    public class GameCycle {
        private SpawnManager spawnManager;
        private GameActor player;
        private GameRules rules;
        private Vector3 playerSpawnPosition;
        private ActorsAggregator actorsAggregator;
        private CollisionSystem collisionSystem;
        private bool alreadyWin;
        private bool alreadyStarted = true;

        private List<GameActor> presettedActors;

        public event Func<Task> OnWin;

        public bool CanShoot => alreadyStarted && !alreadyWin && player != null;

        public GameCycle(SpawnManager spawnManager, IPointerEventHadler pointerUpHandler, GameRules rules, ActorsAggregator actorsAggregator) {
            this.actorsAggregator = actorsAggregator;
            this.rules = rules;
            this.spawnManager = spawnManager;

            pointerUpHandler.OnPointerEvent += OnTapComplete;
            playerSpawnPosition = SpawnHelpers.GetPlayerSpawnPosition();

            collisionSystem = new CollisionSystem(actorsAggregator, rules);
            collisionSystem.OnMerge += OnMerge;
            collisionSystem.OnCollide += OnCollide;
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
            await actor.AnimateSpawn(true);
            
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
            var actor = spawnManager.SpawnOnMerge(position, grade);
            PrepareActor(actor);
            actor.SwitchGravity(true);

            await actor.AnimateSpawn();

            if (!alreadyWin && grade >= rules.WinGrade) {
                alreadyWin = true;
                var startTime = Time.time;
                Debug.LogErrorFormat("OnMerge A {0}", Time.time);
                await OnWin?.Invoke();

                await startTime.DoTimer(5f, 1f, 0f, x => { actor.SharedMaterial.SetFloat("_IsColorized", x); }).OnComplete(() => { Debug.LogErrorFormat("OnMerge B {0}", Time.time); }).AsyncWaitForCompletion();                
            }
        }

        private void OnCollide(Vector3 position, Color color1, Color color2, Vector2 sizes) {
            spawnManager.SpwanCollideEffect(position, color1, color2, sizes);
        }

        private void PrepareActor(GameActor actor) {
            actor.gameObject.SetActive(true);
            actor.OnTrigger += collisionSystem.OnTrigger;
            actorsAggregator.AddComponent(actor.ColliderId, actor);
        }
    }
}
