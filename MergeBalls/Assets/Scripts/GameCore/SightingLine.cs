using BiniGames.GameCore.Spawn;
using BiniGames.Input;
using UnityEngine;

namespace BiniGames.GameCore {
    public class SightingLine {
        private LineRenderer lineRenderer;
        private GameRules gameRules;
        private GameCycle gameCycle;

        public SightingLine(GameRules gameRules, GameCycle gameCycle, LineRenderer lineRenderer, IPointerEventHadler pointerDownHandler, IPointerEventHadler pointerUpHandler, IPointerEventHadler pointerMoveHandler) {
            this.gameRules = gameRules;
            this.lineRenderer = lineRenderer;
            this.gameCycle = gameCycle;

            pointerDownHandler.OnPointerEvent += OnPointerDown;
            pointerUpHandler.OnPointerEvent += OnPointerUp;
            pointerMoveHandler.OnPointerEvent += OnPointerMove;
            lineRenderer.SetPosition(0, SpawnHelpers.GetPlayerSpawnPosition());
        }

        private void OnPointerDown(Vector2 point) {
            if (!gameCycle.CanShoot) return;

            lineRenderer.enabled = true;            
            lineRenderer.SetPosition(1, GetWorldPosition(lineRenderer.GetPosition(0), point));
        }

        private void OnPointerUp(Vector2 point) {
            lineRenderer.enabled = false;
        }

        private void OnPointerMove(Vector2 point) {
            lineRenderer.SetPosition(1, GetWorldPosition(lineRenderer.GetPosition(0), point));
        }

        private Vector3 GetWorldPosition(Vector3 startPosition, Vector2 point) {
            var worldPosition = Camera.main.ScreenToWorldPoint(point);
            worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0);
            var direction = worldPosition - startPosition;
            return startPosition + direction.normalized * gameRules.SightLineLenght;
        }
    }
}
