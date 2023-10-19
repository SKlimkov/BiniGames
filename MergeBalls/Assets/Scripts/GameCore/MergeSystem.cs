using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public class MergeSystem {
        public event Action<Vector3, int> OnMerge;
        private Dictionary<int, Collision2D> collisions;
        private ActorsAggregator actorsAggregator;

        public MergeSystem(ActorsAggregator actorsAggregator) {
            this.actorsAggregator = actorsAggregator;
            collisions = new Dictionary<int, Collision2D>();
        }

        public void OnCollision(Collision2D collision) {
            var sender = collision.collider.GetInstanceID();
            var other = collision.otherCollider.GetInstanceID();
            if (!actorsAggregator.HasActor(other) || !actorsAggregator.HasActor(sender)) return;

            if (actorsAggregator.GetGradeByColliderId(other) != actorsAggregator.GetGradeByColliderId(sender)) return;

            if (actorsAggregator.IsMarkedToKill(sender) || actorsAggregator.IsMarkedToKill(other)) return;

            Debug.LogErrorFormat("OnCollision B {0}, {1}", sender, other);

            if (!collisions.ContainsKey(sender)) collisions.Add(other, collision);
            else Merge(collision, collisions);
        }

        private void Merge(Collision2D collision, Dictionary<int, Collision2D> collisions) {
            var otherId = collision.otherCollider.GetInstanceID();
            collisions.Remove(otherId);
            var actor1 = collision.otherCollider.gameObject.GetComponent<GameActor>();
            var actor2 = collision.collider.GetComponent<GameActor>();
            var direction = actor1.transform.position - actor2.transform.position;
            var distance = direction.magnitude;
            var spawnPosition = actor1.transform.position + direction.normalized * distance / 2;
            OnMerge?.Invoke(spawnPosition, actor1.Key + 1);

            actor1.AnimateDeath(spawnPosition);
            actor1.OnCollide -= OnCollision;
            actorsAggregator.RemoveActor(actor1);

            
            actor2.AnimateDeath(spawnPosition);
            actor2.OnCollide -= OnCollision;
            actorsAggregator.RemoveActor(actor2);
        }
    }
}
