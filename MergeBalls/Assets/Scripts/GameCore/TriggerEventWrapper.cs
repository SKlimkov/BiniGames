using System;
using UnityEngine;

namespace BiniGames.GameCore {
    public class TriggerEventWrapper : MonoBehaviour {
        public event Action<Collider2D> OnTrigger;

        private void OnTriggerEnter2D(Collider2D collision) {
            OnTrigger?.Invoke(collision);
        }
    }
}
