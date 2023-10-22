using System.Collections.Generic;
using UnityEngine;

namespace BiniGames.GameCore {
    public class SoftCollider : MonoBehaviour {
        private CircleCollider2D mainCollider;
        public int ColliderId { get { return mainCollider.GetInstanceID(); } }
        public List<CircleCollider2D> SoftColliders { get; private set; }
        public void Initialize(CircleCollider2D mainCollider, List<CircleCollider2D> softColliders) {
            this.mainCollider = mainCollider;
            SoftColliders = softColliders;
        }
    }
}
