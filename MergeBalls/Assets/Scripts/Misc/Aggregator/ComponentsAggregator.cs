using System.Collections.Generic;
using System;
using UnityEngine;

namespace BiniGames {
    public abstract class ComponentsAggregator {
        public abstract bool HasComponent(int id);
    }

    public sealed class ComponentsAggregator<TComponent> : ComponentsAggregator where TComponent : Component {
        private Dictionary<int, TComponent> aggregation;

        public ComponentsAggregator() {
            aggregation = new Dictionary<int, TComponent>();
        }

        public void AddComponent(int id, TComponent actor) {
            var isSuccess = aggregation.TryAdd(id, actor);
            if (!isSuccess) throw new System.Exception($"Component {typeof(TComponent)} with {id} instance id already exists!");
        }

        public void RemoveComponent(int id) {
            if (aggregation.ContainsKey(id)) aggregation.Remove(id);
            else throw new System.Exception($"Can't find component {typeof(TComponent)} with {id} instance id!");
        }

        public override bool HasComponent(int id) {
            return aggregation.ContainsKey(id);
        }

        public TComponent GetComponent(int id) {
            var isSuccess = aggregation.TryGetValue(id, out var actor);
            if (!isSuccess) throw new Exception($"Can't find component {typeof(TComponent)} with {id} instance id!");
            return actor;
        }
    }
}
