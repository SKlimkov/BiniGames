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

        private async void Awake() {
            var spawnManager = new SpawnManager(actorPrefabs, gameRules, ballsRoot, poolRoot);
            var actorsAggregator = new ActorsAggregator();
            var gameCycle = new GameCycle(spawnManager, pointerUpHadler, gameRules, actorsAggregator);
            var sightLine = new SightingLine(gameRules, lineRenderer, pointerDownHadler, pointerUpHadler, pointerMoveHadler);

            await Task.Delay(100);
            uIManager.HideLoadingScreen();

            await gameCycle.PrepareField();

            await Task.Delay(100);
        }
    }
}
