using BiniGames.Effects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public class CollisionSystem {
        public event Action<Vector3, int> OnMerge;
        public event Action<Vector3, Color, Color, Vector2> OnCollide;

        private Dictionary<int, Collision2D> collisions;
        private ActorsAggregator actorsAggregator;
        private GameRules gameRules;

        public CollisionSystem(ActorsAggregator actorsAggregator, GameRules gameRules) {
            this.actorsAggregator = actorsAggregator;
            this.gameRules = gameRules;
            collisions = new Dictionary<int, Collision2D>();
        }

        public void OnCollision(Collision2D collision) {
            var sender = collision.collider.GetInstanceID();
            var other = collision.otherCollider.GetInstanceID();
            if (!actorsAggregator.HasComponent<GameActor>(other) || !actorsAggregator.HasComponent<GameActor>(sender)) return;

            //Prevent double call for same collision from different balls
            if (!collisions.ContainsKey(sender)) {
                collisions.Add(other, collision);
                return;
            }

            collisions.Remove(collision.otherCollider.GetInstanceID());
            collisions.Remove(collision.collider.GetInstanceID());

            if (collision.relativeVelocity.sqrMagnitude >= gameRules.SqrMinRelativeVelocityForEffects) PlayCollideEffect(collision);

            if (actorsAggregator.GetGradeByColliderId(other) != actorsAggregator.GetGradeByColliderId(sender)) return;

            if (actorsAggregator.IsMarkedToKill(sender) || actorsAggregator.IsMarkedToKill(other)) return;

            Merge(collision);
        }

        private void PlayCollideEffect(Collision2D collision) {
            var actor1 = collision.otherCollider.gameObject.GetComponent<GameActor>();
            var actor2 = collision.collider.GetComponent<GameActor>();
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor2.transform.position + direction.normalized * distance / 2;
            OnCollide?.Invoke(spawnPosition, actor1.Color, actor2.Color, new Vector2((collision.otherCollider as CircleCollider2D).radius, (collision.collider as CircleCollider2D).radius));
        }

        private void Merge(Collision2D collision) {
            var otherId = collision.otherCollider.GetInstanceID();
            var actor1 = collision.otherCollider.gameObject.GetComponent<GameActor>();
            var actor2 = collision.collider.GetComponent<GameActor>();
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor2.transform.position + direction.normalized * distance / 2;
            OnMerge?.Invoke(spawnPosition, actor1.Key + 1);

            actor1.AnimateDeath(spawnPosition);
            actor1.OnCollide -= OnCollision;
            actorsAggregator.RemoveComponent<GameActor>(actor1.ColliderId);
            actorsAggregator.RemoveComponent<Rigidbody2D>(actor1.ColliderId);

            actor2.AnimateDeath(spawnPosition);
            actor2.OnCollide -= OnCollision;
            actorsAggregator.RemoveComponent<GameActor>(actor2.ColliderId);
            actorsAggregator.RemoveComponent<Rigidbody2D>(actor2.ColliderId);
        }
    }
}
