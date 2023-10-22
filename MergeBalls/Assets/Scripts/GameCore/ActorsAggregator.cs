using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public static class AggregatorHelpers {
        public static int GetGradeByColliderId(this ActorsAggregator actorsAggregator, int id) {
            return actorsAggregator.GetComponent<GameActor>(id).Key;
        }

        public static bool IsMarkedToKill(this ActorsAggregator actorsAggregator, int id) {
            return actorsAggregator.GetComponent<GameActor>(id).IsMarkedToKill;
        }

        public static void AddSoftCollider(this ActorsAggregator actorsAggregator, SoftCollider softCollider) {
            for (var i = 0; i < softCollider.SoftColliders.Count; i++) {
                actorsAggregator.AddComponent(softCollider.SoftColliders[i].GetInstanceID(), softCollider);
            }
        }

        public static void RemoveSoftCollider(this ActorsAggregator actorsAggregator, SoftCollider softCollider) {
            for (var i = 0; i < softCollider.SoftColliders.Count; i++) {
                actorsAggregator.RemoveComponent<SoftCollider>(softCollider.SoftColliders[i].GetInstanceID());
            }
        }
    }

    public class ActorsAggregator {
        private Dictionary<Type, ComponentsAggregator> aggregators;

        public ActorsAggregator() {
            aggregators = new Dictionary<Type, ComponentsAggregator> {
                { typeof(GameActor), new ComponentsAggregator<GameActor>() },
                { typeof(SoftCollider), new ComponentsAggregator<SoftCollider>() }
            };
        }

        public void AddComponent<TComponent>(int id, TComponent component) where TComponent : Component {
            var isSucces = aggregators.TryGetValue(typeof(TComponent), out var aggregator);
            if (!isSucces) throw new Exception($"Can't find aggregator for type {typeof(TComponent)}");

            (aggregator as ComponentsAggregator<TComponent>).AddComponent(id, component);
        }

        public void RemoveComponent<TComponent>(int id) where TComponent : Component {
            var isSucces = aggregators.TryGetValue(typeof(TComponent), out var aggregator);
            if (!isSucces) throw new Exception($"Can't find aggregator for type {typeof(TComponent)}");

            (aggregator as ComponentsAggregator<TComponent>).RemoveComponent(id);
        }

        public bool HasComponent<TComponent>(int id) where TComponent : Component {
            var isSucces = aggregators.TryGetValue(typeof(TComponent), out var aggregator);
            return isSucces ? aggregator.HasComponent(id) : throw new Exception($"Can't find aggregator for type {typeof(TComponent)}");
        }

        public TComponent GetComponent<TComponent>(int id) where TComponent : Component {
            var isSucces = aggregators.TryGetValue(typeof(TComponent), out var aggregator);
            return isSucces ? (aggregator as ComponentsAggregator<TComponent>).GetComponent(id) : throw new Exception($"Can't find aggregator for type {typeof(TComponent)}");
        }
    }
}
