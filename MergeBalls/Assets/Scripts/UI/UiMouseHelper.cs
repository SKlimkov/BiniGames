using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BiniGames.UI {
    public class UiMouseHelper : MonoBehaviour {
        [SerializeField]
        private bool needDebug = true;
        private Vector2 boxSize;

        private void Awake() {
            boxSize = new Vector2(Mathf.Clamp(Screen.width * 0.4f, 100, 400), Mathf.Clamp(Screen.height * 0.05f, 30, 200));
        }

        private void OnGUI() {
            if (needDebug) {
                var eventData = new PointerEventData(EventSystem.current);
                eventData.position = UnityEngine.Input.mousePosition;
                var raycasts = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, raycasts);
                if (raycasts.Count == 0) return;

                var result = raycasts.First();
                if (result.gameObject == null) return;

                GUI.Box(new Rect(0, 0, boxSize.x, boxSize.y), $"{result.gameObject.name}, {result.gameObject.transform.GetInstanceID()}");
            }
        }
    }
}
