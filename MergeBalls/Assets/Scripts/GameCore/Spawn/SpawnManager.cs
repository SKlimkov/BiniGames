using BiniGames.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore.Spawn {
    public class SpawnManager {
        private GameRules gameRules;
        private Transform ballsRoot, poolRoot;
        private PolyMonoObjectPool<GameActor> polyMonoObjectPool;
        private MonoObjectPool<CollideEffect> effectPool;
        private CollideEffect effectPrefab;

        private int maxBallGradeOnField;
        private List<GameActor> startBallsCache;
        private Vector3 playerSpawnPosition;

        public Material SharedMaterial { get; private set; }

        public SpawnManager(GameActor[] actorPrefabs, CollideEffect effectPrefab, GameRules gameRules, Transform ballsRoot, Transform poolRoot) {
            this.gameRules = gameRules;
            this.ballsRoot = ballsRoot;
            this.poolRoot = poolRoot;
            this.effectPrefab = effectPrefab;

            var actorFactory = new ActorFactory(actorPrefabs);
            polyMonoObjectPool = new PolyMonoObjectPool<GameActor>(actorFactory, poolRoot);
            effectPool = new MonoObjectPool<CollideEffect>(poolRoot);
            startBallsCache = new List<GameActor>();
            playerSpawnPosition = SpawnHelpers.GetPlayerSpawnPosition();

            actorPrefabs[0].SharedMaterial.SetFloat("_IsColorized", 1f);
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
                startBallsCache.Add(actor);
            }

            return startBallsCache;
        }

        public GameActor SpawnOnMerge(Vector3 position, int grade) {
            return GetGameActor(grade, position, ballsRoot);
        }

        public void SpwanCollideEffect(Vector3 position, Color color1, Color color2, Vector2 sizes) {
            var effect = effectPool.Pop(effectPrefab, position, ballsRoot);
            effect.Play(color1, color2, sizes);
            effect.OnComplete += OnCollideEffectComplete;
        }

        private void OnCollideEffectComplete(CollideEffect effect) {
            effect.OnComplete -= OnCollideEffectComplete;
            effectPool.Pool(effect);
        }

        private GameActor GetGameActor(int grade, Vector3 position, Transform root, bool activate = true) {
            var result = polyMonoObjectPool.Pop(grade, root);
            result.transform.position = position;
            result.gameObject.SetActive(activate);
            result.OnDeath += OnActorDeath;
            maxBallGradeOnField = grade > maxBallGradeOnField ? grade : maxBallGradeOnField;
            return result;
        }

        private void OnActorDeath(GameActor gameActor) {
            gameActor.OnDeath -= OnActorDeath;
            polyMonoObjectPool.Pool(gameActor, poolRoot);
        }
    }
}
