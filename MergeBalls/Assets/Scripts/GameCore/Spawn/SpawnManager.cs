using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore.Spawn {
    public class SpawnManager {
        private GameRules gameRules;
        private Transform ballsRoot, poolRoot;
        private PolyMonoObjectPool<GameActor> polyMonoObjectPool;

        private int maxBallGradeOnField;
        private List<GameActor> startBallsCache;
        private Vector3 playerSpawnPosition;

        public SpawnManager(GameActor[] actorPrefabs, GameRules gameRules, Transform ballsRoot, Transform poolRoot) {
            this.gameRules = gameRules;
            this.ballsRoot = ballsRoot;
            this.poolRoot = poolRoot;

            var actorFactory = new ActorFactory(actorPrefabs);
            polyMonoObjectPool = new PolyMonoObjectPool<GameActor>(actorFactory);
            startBallsCache = new List<GameActor>();
            playerSpawnPosition = SpawnHelpers.GetPlayerSpawnPosition();
        }

        public GameActor SpawnPlayer() {
            var grade = gameRules.PlayerSpawnType == GameRules.EPlayerSpawnType.RandomGrade ? Random.Range(0, Mathf.Clamp(maxBallGradeOnField, 0, gameRules.MaxPlayerBallGrade) + 1) : gameRules.PlayerGrade;
            return GetGameActor(grade, playerSpawnPosition, ballsRoot);
        }

        public List<GameActor> SpawnPresettedActors() {
            var count = Random.Range(gameRules.MinStartBallsCount, gameRules.MaxStartBallsCount + 1);
            startBallsCache.Clear();
            var borderSize = 1f;
            var screenWidth = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x - borderSize;
            var leftOffset = 0 - screenWidth / 2f;
            for (var i = 0; i < count; i++) {
                var xPosition = Random.Range(leftOffset, leftOffset + screenWidth);
                var position = new Vector3(xPosition, 0, 0);
                var grade = Random.Range(0, gameRules.MaxStartBallsGrade + 1);
                var actor = GetGameActor(grade, position, ballsRoot, false);
                maxBallGradeOnField = actor.Key > maxBallGradeOnField ? actor.Key : maxBallGradeOnField;
                startBallsCache.Add(actor);
            }

            return startBallsCache;
        }

        public GameActor SpawnOnMerge(Vector3 position, int grade) {
            return GetGameActor(grade, position, ballsRoot);
        }

        private GameActor GetGameActor(int grade, Vector3 position, Transform root, bool activate = true) {
            var result = polyMonoObjectPool.Pop(grade, root);
            result.transform.position = position;
            result.gameObject.SetActive(activate);
            result.OnDeath += OnActorDeath;
            return result;
        }

        private void OnActorDeath(GameActor gameActor) {
            gameActor.OnDeath -= OnActorDeath;
            polyMonoObjectPool.Pool(gameActor, poolRoot);
        }
    }
}