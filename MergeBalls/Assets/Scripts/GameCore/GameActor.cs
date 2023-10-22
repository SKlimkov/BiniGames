using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BiniGames.GameCore {
    public class GameActor : MonoBehaviour, IPolyPoolable {
        [SerializeField] private int grade;
        [SerializeField] private new CircleCollider2D collider2D;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Color color;

        [SerializeField] private bool isSoft;
        [SerializeField] private List<Rigidbody2D> softRigidBodies;
        [SerializeField] private List<CircleCollider2D> softColliders;
        [SerializeField] private SoftCollider softCollider;

        private HashSet<int> softCollidersIdSet;

        public event Action<GameActor> OnDeath;
        public event Action<Collision2D> OnCollision;
        public event Action<Collider2D, Collider2D> OnTrigger;

        private float defaultScaleFactor = 0.01f;

        public Vector3 Velocity { get { return isSoft ? GetAverageVelocity(softRigidBodies) : rigidbody2D.velocity; } }
        public Color Color => color;
        public int Key => grade;
        public int ColliderId => collider2D.GetInstanceID();
        public bool IsMarkedToKill { get; private set; }
        public SoftCollider SoftCollider => softCollider;

        private void Awake() {
            if (isSoft) SetActionToComponentList(softRigidBodies, (x) => {
                x.mass = PhysicsHelpers.RadiusToMass(collider2D.radius);
            });
            else rigidbody2D.mass = PhysicsHelpers.RadiusToMass(collider2D.radius);

            if (!isSoft) return;

            softCollidersIdSet = new HashSet<int>();
            for (var i = 0; i < softColliders.Count; i++) {
                softCollidersIdSet.Add(softColliders[i].GetInstanceID());
            }
        }

        public void OnPop() {
            SwitchGravity(false);
            IsMarkedToKill = false;
            trailRenderer.emitting = true;
            trailRenderer.widthMultiplier = collider2D.radius * 2f;
            if (isSoft) SetActionToComponentList(softRigidBodies, (x) => {
                x.constraints = RigidbodyConstraints2D.None;
            });
            else rigidbody2D.constraints = RigidbodyConstraints2D.None;

            if (isSoft) softCollider.Initialize(collider2D, softColliders);
        }

        public void OnPool() {
            SwitchGravity(false);
            gameObject.SetActive(false);
            transform.localScale = Vector3.one * defaultScaleFactor;
        }

        public void Fire(Vector3 direction) {
            if (isSoft) SetActionToComponentList(softRigidBodies, (x) => {
                x.AddForce(direction * x.mass, ForceMode2D.Impulse);
            });
            else rigidbody2D.AddForce(direction * rigidbody2D.mass, ForceMode2D.Impulse);

        }

        public void SwitchGravity(bool active) {
            collider2D.enabled = active;
            if (isSoft) SetActionToComponentList(softRigidBodies, (x) => {
                x.gravityScale = active ? 1 : 0;
            });
            else rigidbody2D.gravityScale = active ? 1 : 0;
        }

        public async Task AnimateSpawn() {
            if (isSoft) {
                for (var i = 0; i < softColliders.Count; i++) softColliders[i].gameObject.SetActive(true);
            }
            await transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }

        public void AnimateCollision() {
            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 0.8f, 0.05f).SetEase(Ease.InOutCubic))
                .Append(transform.DOScale(Vector3.one * 1f, 0.05f).SetEase(Ease.InOutCubic));
        }

        public async void AnimateDeath(Vector3 mergePosition) {
            trailRenderer.emitting = false;
            if (isSoft) {
                for (var i = 0; i < softColliders.Count; i++) softColliders[i].gameObject.SetActive(true);
                SetActionToComponentList(softRigidBodies, (x) => {
                    x.constraints = RigidbodyConstraints2D.FreezeAll;
                });
            }
            else rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
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
            OnCollision?.Invoke(collision);
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            return;
            if (softCollidersIdSet.Contains(collider.GetInstanceID()) || IsMarkedToKill) return;

            OnTrigger?.Invoke(collider2D, collider);
        }

        private void SetActionToComponentList<TComponent>(List<TComponent> list, Action<TComponent> action) {
            for (var i = 0; i < list.Count; i++) {
                var entry = list[i];
                action.Invoke(entry);
            }
        }

        private Vector3 GetAverageVelocity(List<Rigidbody2D> rigidBodies) {
            var result = Vector2.zero;
            for (var i = 0; i < rigidBodies.Count; i++) {
                var rb = rigidBodies[i];
                result += rb.velocity;
            }

            return result * (1f / rigidBodies.Count);
        }
    }
}
