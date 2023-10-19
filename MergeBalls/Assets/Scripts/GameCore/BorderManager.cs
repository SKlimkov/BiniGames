using UnityEngine;

namespace BiniGames.GameCore.Borders {
    public class BorderManager : MonoBehaviour {
        [SerializeField] private bool showBorders;
        private BoxCollider2D[] borders;

        private void Awake() {
            borders = FindObjectsOfType<BoxCollider2D>();
            var screenSize = new Vector2(Screen.width, Screen.height);
            var downLeftCorner = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            var upRightCorner = Camera.main.ScreenToWorldPoint(screenSize);
            var borderHalfSize = 0.5f;

            //set sizes and positions for borders
            for (var i = 0; i < borders.Length; i++) {
                var isOdd = MathHelpers.IsOdd(i);
                var isFirstHalf = i < borders.Length / 2;
                var border = borders[i];

                border.transform.position = new Vector3(
                    isOdd ? isFirstHalf ? downLeftCorner.x - borderHalfSize : upRightCorner.x + borderHalfSize : 0,
                    isOdd ? 0 : isFirstHalf ? downLeftCorner.y - borderHalfSize : upRightCorner.y + borderHalfSize,
                    0);

                border.transform.localScale = new Vector3(
                    isOdd ? 1 : upRightCorner.x * 2,
                    isOdd ? upRightCorner.y * 2 : 1,
                    1);

                border.GetComponent<SpriteRenderer>().enabled = showBorders;
            }
        }
    }
}
