using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public class ActorsAggregator {
        private Dictionary<int, GameActor> actors;

        public ActorsAggregator() {
            actors = new Dictionary<int, GameActor>();
        }

        public void AddActor(GameActor actor) {
            var isSuccess = actors.TryAdd(actor.ColliderId, actor);
            if (!isSuccess) throw new System.Exception($"Actor with {actor.ColliderId} instance id already exists!");
        }

        public void RemoveActor(GameActor actor) {
            if (actors.ContainsKey(actor.ColliderId)) actors.Remove(actor.ColliderId);
            else throw new System.Exception($"Can't find actor with {actor.ColliderId} instance id!");
        }

        public bool HasActor(int id) {
            return actors.ContainsKey(id);
        }

        public int GetGradeByColliderId(int id) {
            return GetActor(id).Key;
        }

        public bool IsMarkedToKill(int id) {
            return GetActor(id).IsMarkedToKill;
        }

        public GameActor GetActor(int id) {
            var isSuccess = actors.TryGetValue(id, out var actor);
            if (!isSuccess) throw new System.Exception($"Can't find actor with {actor.GetInstanceID()} instance id!");
            return actor;
        }
    }
}
