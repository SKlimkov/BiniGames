using UnityEngine;

namespace BiniGames.GameCore.Spawn {
    public static class SpawnHelpers  {
        public static Vector3 GetPlayerSpawnPosition() {
            var screenSpawnPosition = new Vector2(Screen.width / 2, Screen.height * 0.9f);
            var worldSpawnPosition = Camera.main.ScreenToWorldPoint(screenSpawnPosition);
            return new Vector3(worldSpawnPosition.x, worldSpawnPosition.y, 0);
        }
    }
}
