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
}
