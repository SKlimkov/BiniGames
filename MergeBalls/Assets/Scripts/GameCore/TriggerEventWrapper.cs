using System;
using UnityEngine;

namespace BiniGames.GameCore {
    public class TriggerEventWrapper : MonoBehaviour {
        public event Action<Collider2D> OnTriggerEnterEvent;
        public event Action<Collider2D> OnTriggerExitEvent;

        private void OnTriggerEnter2D(Collider2D collision) {
            OnTriggerEnterEvent?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision) {
            OnTriggerExitEvent?.Invoke(collision);
        }
    }
}
