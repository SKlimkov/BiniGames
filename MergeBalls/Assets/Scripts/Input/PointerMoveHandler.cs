using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BiniGames.Input {
    public class PointerMoveHandler : MonoBehaviour, IPointerEventHadler, IPointerMoveHandler {
        public event Action<Vector2> OnPointerEvent;
        public void OnPointerMove(PointerEventData eventData) {
            //Debug.LogErrorFormat("OnPointerMove {0}", eventData.position);

            var position = eventData.position;
            OnPointerEvent?.Invoke(position);
        }
    }
}
