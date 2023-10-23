using UnityEngine;

namespace BiniGames.CG {
    [RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
    public class SpriteRendererGrayScaleTurnable : MonoBehaviour {
        [Range(0, 1)]
        [SerializeField] private float gray;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public float Gray { get { return gray; } set { gray = SetGray(value); } }

        private float SetGray(float gray) {
            spriteRenderer.sharedMaterial.SetFloat("_IsColorized", gray);
            return gray;
        }

        private void OnValidate() {
            Gray = gray;
        }
    }
}
