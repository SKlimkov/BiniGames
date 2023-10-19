using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BiniGames.Input {
    public class PointerDownHandler : MonoBehaviour, IPointerEventHadler, IPointerDownHandler {
        public event Action<Vector2> OnPointerEvent;

        public void OnPointerDown(PointerEventData eventData) {
            var position = eventData.position;
            OnPointerEvent?.Invoke(position);
        }
    }
}
