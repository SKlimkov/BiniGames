using UnityEngine;

namespace BiniGames {
    public static class PhysicsHelpers {
        public static float RadiusToMass(float radius) {
            return Mathf.PI * radius * radius;
        }
    }
}
