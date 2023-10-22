using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class EightPointCircledSetter : MonoBehaviour {
    private enum EVeryName { Center, TopRight, Top, TopLeft, Left, BottomLeft, Bottom, BottomRight, Right, };

    [SerializeField] private float radius;

    private const int pointsCount = 8;

    [EasyButtons.Button]
    private void SetVertises() {
        var childCount = transform.childCount;
        for (var i = 0; i < pointsCount + 1; i++) {
            var child = transform.GetChild(i);
            child.gameObject.name = ((EVeryName)i).ToString();
            if (i == 0) {
                child.localPosition = Vector3.zero;
                continue;
            }

            var angle = i * Mathf.PI * 2f / pointsCount;
            var offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);            
            child.localPosition = offset;
        }
    }

    [EasyButtons.Button]
    private void CalculatePositions() {
        var sr = GetComponent<SpriteRenderer>();
        var fullRadius = sr.sprite.rect.height;
        var radius = fullRadius / 2 * 0.6f;
        Debug.LogErrorFormat("CalculatePositions {0}: {1}", fullRadius, radius);
        var center = new Vector2 (fullRadius / 2f, fullRadius / 2f);
        for (var i = 0; i < 8; i++) {
            var angle = i * Mathf.PI * 2f / pointsCount;
            var offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Debug.LogErrorFormat("{0}: {1}", i + 1, center + offset);
        }
    }
}
