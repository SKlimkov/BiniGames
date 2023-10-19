using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore.Spawn {
    public class ActorFactory : IMonoPoolableFactory<GameActor> {
        private Dictionary<int, GameActor> prefabs;

        public ActorFactory(GameActor[] prefabList) {
            prefabs = new Dictionary<int, GameActor>();
            for (var i = 0; i < prefabList.Length; i++) {
                var prefab = prefabList[i];
                var isSuccess = prefabs.TryAdd(prefab.Key, prefab);
                if (!isSuccess) throw new System.Exception($"Prefab with {prefab.Key} grade already exists!");
            }
        }

        public GameActor CreateInstance(int key, Transform parent) {
            var isFound = prefabs.TryGetValue(key, out var prefab);
            if (!isFound) throw new System.Exception($"Can't find prefab for {key} grade!");
            return GameObject.Instantiate(prefab, new Vector3(), Quaternion.identity, parent);
        }
    }
}
