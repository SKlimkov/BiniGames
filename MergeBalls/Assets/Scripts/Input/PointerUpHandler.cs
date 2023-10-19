using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BiniGames.Input {
    public class PointerUpHandler : MonoBehaviour, IPointerEventHadler, IPointerUpHandler {
        public event Action<Vector2> OnPointerEvent;
        public void OnPointerUp(PointerEventData eventData) {
            var position = eventData.position;
            OnPointerEvent?.Invoke(position);
        }
    }
}
