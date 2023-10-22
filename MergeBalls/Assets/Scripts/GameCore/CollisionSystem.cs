using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public class CollisionSystem {
        public struct CollissionData {
            public Collider2D Other;
            public Collider2D Sender;
        }

        public event Action<Vector3, int> OnMerge;
        public event Action<Vector3, Color, Color, Vector2> OnCollide;

        private Dictionary<int, CollissionData> collisions;
        private ActorsAggregator actorsAggregator;
        private GameRules gameRules;

        public CollisionSystem(ActorsAggregator actorsAggregator, GameRules gameRules) {
            this.actorsAggregator = actorsAggregator;
            this.gameRules = gameRules;
            collisions = new Dictionary<int, CollissionData>();
        }

        public void OnCollision(Collision2D collision) {
            var sender = collision.collider;
            var other = collision.otherCollider;
            TryMerge(sender, other, collision.relativeVelocity);
        }

        public void OnTrigger(Collider2D sender, Collider2D other) {
            var softBody1 = actorsAggregator.GetComponent<SoftCollider>(sender.GetInstanceID());
            var softBody2 = actorsAggregator.GetComponent<SoftCollider>(other.GetInstanceID());

            /*if (!actorsAggregator.HasComponent<GameActor>(softBody1.ColliderId) || !actorsAggregator.HasComponent<GameActor>(softBody2.ColliderId)) return;*/

            var actor1 = actorsAggregator.GetComponent<GameActor>(softBody1.ColliderId);
            var actor2 = actorsAggregator.GetComponent<GameActor>(softBody2.ColliderId);

            TryMerge(sender, other, actor1.Velocity + actor2.Velocity);
        }

        private void TryMerge(Collider2D sender, Collider2D other, Vector3 relativeVelocity) {
            var senderId = sender.GetInstanceID();
            var otherId = other.GetInstanceID();
            if (!actorsAggregator.HasComponent<GameActor>(otherId) || !actorsAggregator.HasComponent<GameActor>(senderId)) return;

            if (actorsAggregator.IsMarkedToKill(senderId) || actorsAggregator.IsMarkedToKill(otherId)) return;

            if (gameRules.UseSoftPrefabs) {
                var collision = new CollissionData() { Other = other, Sender = sender };
                //Prevent double call for same collision from different balls
                if (!collisions.ContainsKey(senderId)) {
                    collisions.Add(otherId, collision);
                    return;
                }

                collisions.Remove(collision.Other.GetInstanceID());
                collisions.Remove(collision.Sender.GetInstanceID());
            }

            if (relativeVelocity.sqrMagnitude >= gameRules.SqrMinRelativeVelocityForEffects) PlayCollideEffect(other, sender);

            if (actorsAggregator.GetGradeByColliderId(otherId) != actorsAggregator.GetGradeByColliderId(senderId)) return;

            Merge(other, sender);
        }

        private void PlayCollideEffect(Collider2D collider1, Collider2D collider2) {
            var actor1 = collider1.gameObject.GetComponent<GameActor>(); //other
            var actor2 = collider2.GetComponent<GameActor>();
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor2.transform.position + direction.normalized * distance / 2;
            OnCollide?.Invoke(spawnPosition, actor1.Color, actor2.Color, new Vector2((collider1 as CircleCollider2D).radius, (collider2 as CircleCollider2D).radius));
        }

        private void Merge(Collider2D collider1, Collider2D collider2) {
            var otherId = collider1.GetInstanceID();
            var actor1 = collider1.gameObject.GetComponent<GameActor>();
            var actor2 = collider2.GetComponent<GameActor>();
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor2.transform.position + direction.normalized * distance / 2;
            OnMerge?.Invoke(spawnPosition, actor1.Key + 1);

            actor1.AnimateDeath(spawnPosition);
            actor1.OnCollision -= OnCollision;
            actor1.OnTrigger -= OnTrigger;
            actorsAggregator.RemoveComponent<GameActor>(actor1.ColliderId);
            if (gameRules.UseSoftPrefabs) actorsAggregator.RemoveSoftCollider(actor1.SoftCollider);

            actor2.AnimateDeath(spawnPosition);
            actor2.OnCollision -= OnCollision;
            actor2.OnTrigger -= OnTrigger;
            actorsAggregator.RemoveComponent<GameActor>(actor2.ColliderId);
            if (gameRules.UseSoftPrefabs) actorsAggregator.RemoveSoftCollider(actor2.SoftCollider);
        }
    }
}
