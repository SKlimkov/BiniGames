using System;
using UnityEngine;

namespace BiniGames.Input {
    public interface  IPointerEventHadler {
        event Action<Vector2> OnPointerEvent;
    }
}
