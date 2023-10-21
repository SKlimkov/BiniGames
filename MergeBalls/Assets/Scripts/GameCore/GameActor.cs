using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BiniGames.GameCore {
    public class GameActor : MonoBehaviour, IPolyPoolable {
        [SerializeField] private int grade;
        [SerializeField] private new CircleCollider2D collider2D;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Color color;

        public event Action<GameActor> OnDeath;
        public event Action<Collision2D> OnCollide;

        public Color Color => color;
        public Rigidbody2D Rigidbody => rigidbody2D;
        public int Key => grade;
        public int ColliderId => collider2D.GetInstanceID();
        public bool IsMarkedToKill { get; private set; }

        private void Awake() {
            rigidbody2D.mass = PhysicsHelpers.RadiusToMass(collider2D.radius);
        }

        public void OnPop() {
            IsMarkedToKill = false;
            collider2D.enabled = true;
            trailRenderer.emitting = true;
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
        }

        public void OnPool() {
            SwitchGravity(false);
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }

        public void Fire(Vector3 direction) {
            rigidbody2D.AddForce(direction * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void SwitchGravity(bool active) {
            collider2D.enabled = active;
            rigidbody2D.gravityScale = active ? 1 : 0;
        }

        public async Task AnimateSpawn() {
            await transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }

        public void AnimateCollision() {
            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 0.8f, 0.05f).SetEase(Ease.InOutCubic))
                .Append(transform.DOScale(Vector3.one * 1f, 0.05f).SetEase(Ease.InOutCubic));
        }

        public async void AnimateDeath(Vector3 mergePosition) {
            trailRenderer.emitting = false;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            IsMarkedToKill = true;
            collider2D.enabled = false;
            await DOTween.Sequence()
                .Append(transform.DOScale(0, 0.2f))
                .Join(transform.DOMove(mergePosition, 0.3f))
                .AsyncWaitForCompletion();

            Kill();
        }

        public void Kill() {
            OnDeath?.Invoke(this);
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            OnCollide?.Invoke(collision);
        }
    }
}
