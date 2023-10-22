using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public class CollisionSystem {
        public event Action<Vector3, int> OnMerge;
        public event Action<Vector3, Color, Color, Vector2> OnCollide;

        private HashSet<int> collisions;
        private ActorsAggregator actorsAggregator;
        private GameRules gameRules;

        public CollisionSystem(ActorsAggregator actorsAggregator, GameRules gameRules) {
            this.actorsAggregator = actorsAggregator;
            this.gameRules = gameRules;
            collisions = new HashSet<int>();
        }

        public void OnTrigger(Collider2D sender, Collider2D other) {
            var otherActor = actorsAggregator.GetComponent<GameActor>(other.GetInstanceID());
            if (otherActor.IsMarkedToKill) return;

            Debug.LogFormat("OnTrigger {0}, {1}, {2}", sender.gameObject.name, sender.GetInstanceID(), other.GetInstanceID());

            TryMerge(sender, other, actorsAggregator.GetComponent<GameActor>(sender.GetInstanceID()).Velocity + otherActor.Velocity);
        }

        private void TryMerge(Collider2D sender, Collider2D other, Vector3 relativeVelocity) {
            var senderId = sender.GetInstanceID();
            var otherId = other.GetInstanceID();
            if (!actorsAggregator.HasComponent<GameActor>(otherId) || !actorsAggregator.HasComponent<GameActor>(senderId)) return;

            //Prevent double call for same collision from different balls
            if (!collisions.Contains(senderId)) {
                collisions.Add(otherId);
                return;
            }

            collisions.Remove(senderId);
            collisions.Remove(otherId);

            if (relativeVelocity.sqrMagnitude >= gameRules.SqrMinRelativeVelocityForEffects) PlayCollideEffect(other, sender);

            if (actorsAggregator.GetGradeByColliderId(otherId) != actorsAggregator.GetGradeByColliderId(senderId)) return;

            Merge(other, sender);
        }

        private void PlayCollideEffect(Collider2D collider1, Collider2D collider2) {
            var actor1 = actorsAggregator.GetComponent<GameActor>(collider1.GetInstanceID()); //other
            var actor2 = actorsAggregator.GetComponent<GameActor>(collider2.GetInstanceID());
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor2.transform.position + direction.normalized * distance / 2;
            OnCollide?.Invoke(spawnPosition, actor1.Color, actor2.Color, new Vector2((collider1 as CircleCollider2D).radius, (collider2 as CircleCollider2D).radius));
        }

        private void Merge(Collider2D collider1, Collider2D collider2) {
            var otherId = collider1.GetInstanceID();
            var actor1 = actorsAggregator.GetComponent<GameActor>(collider1.GetInstanceID());
            var actor2 = actorsAggregator.GetComponent<GameActor>(collider2.GetInstanceID());
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor2.transform.position + direction.normalized * distance / 2;
            OnMerge?.Invoke(spawnPosition, actor1.Key + 1);

            actor1.AnimateDeath(spawnPosition);
            actor1.OnTrigger -= OnTrigger;
            actorsAggregator.RemoveComponent<GameActor>(actor1.ColliderId);

            actor2.AnimateDeath(spawnPosition);
            actor2.OnTrigger -= OnTrigger;
            actorsAggregator.RemoveComponent<GameActor>(actor2.ColliderId);
        }
    }
}
