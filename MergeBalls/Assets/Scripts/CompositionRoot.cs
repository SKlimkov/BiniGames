using BiniGames.Effects;
using BiniGames.GameCore;
using BiniGames.GameCore.Spawn;
using BiniGames.Input;
using BiniGames.UI;
using System.Threading.Tasks;
using UnityEngine;

namespace BiniGames {
    public class CompositionRoot : MonoBehaviour {
        [SerializeField] private GameActor[] actorPrefabs;
        [SerializeField] private GameRules gameRules;
        [SerializeField] private Transform ballsRoot;
        [SerializeField] private Transform poolRoot;        
        [SerializeField] private UIManager uIManager;
        [SerializeField] private PointerDownHandler pointerDownHadler;
        [SerializeField] private PointerUpHandler pointerUpHadler;
        [SerializeField] private PointerMoveHandler pointerMoveHadler;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private CollideEffect collideEffectPrefab;

        private bool isGray;
        [EasyButtons.Button]
        private void SetGray() {
            var grayScale = isGray ? 0f : 1f;
        }

        private async void Awake() {
            var spawnManager = new SpawnManager(actorPrefabs, collideEffectPrefab, gameRules, ballsRoot, poolRoot);
            var actorsAggregator = new ActorsAggregator();
            var gameCycle = new GameCycle(spawnManager, pointerUpHadler, gameRules, actorsAggregator);
            var sightLine = new SightingLine(gameRules, gameCycle, lineRenderer, pointerDownHadler, pointerUpHadler, pointerMoveHadler);

            await gameCycle.PrepareField();

            await Task.Delay(200);

            uIManager.HideLoadingScreen();
            gameCycle.OnWin += uIManager.ShowWinScreen;

            await gameCycle.Start();

            await Task.Delay(100);
        }
    }
}
