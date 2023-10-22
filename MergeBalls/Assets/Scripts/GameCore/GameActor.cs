using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BiniGames.GameCore {
    public class GameActor : MonoBehaviour, IPolyPoolable {
        [SerializeField] private int grade;
        [SerializeField] private CircleCollider2D animationViewCollider;
        [SerializeField] private CircleCollider2D body;
        [SerializeField] private CircleCollider2D trigger;
        [SerializeField] private Rigidbody2D animatedViewRbody;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Color color;
        [SerializeField] private GameObject softBody;
        [SerializeField] private Transform additionalRoot;

        private Rigidbody2D[] softRigidBodies;
        private CircleCollider2D[] softColliders;

        public event Action<GameActor> OnDeath;
        public event Action<Collider2D, Collider2D> OnTrigger;

        private float defaultScaleFactor = 0.01f;

        public Vector3 Velocity { get { return GetAverageVelocity(softRigidBodies); } }
        public Color Color => color;
        public int Key => grade;
        public int ColliderId => body.GetInstanceID();
        public bool IsMarkedToKill { get; private set; }

        private void Awake() {
            softRigidBodies = softBody.GetComponentsInChildren<Rigidbody2D>();
            softColliders = softBody.GetComponentsInChildren<CircleCollider2D>();
            GetComponentInChildren<TriggerEventWrapper>().OnTrigger += OnTriggerHandler;
            trigger.radius = animationViewCollider.radius;
            body.radius = animationViewCollider.radius;
            SetActionToComponentList(softRigidBodies, (x) => { x.mass = PhysicsHelpers.RadiusToMass(animationViewCollider.radius); });
            animatedViewRbody.mass = PhysicsHelpers.RadiusToMass(animationViewCollider.radius);
        }

        public void OnPop() {
            SwitchGravity(false);
            IsMarkedToKill = false;
            trailRenderer.emitting = true;
            trailRenderer.widthMultiplier = animationViewCollider.radius * 2f * 0.85f;
            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = RigidbodyConstraints2D.None; });
            additionalRoot.position = Vector3.zero;
        }

        public void OnPool() {
            SwitchGravity(false);
            transform.localScale = Vector3.one * defaultScaleFactor;
        }

        public void Fire(Vector3 direction) {
            SetActionToComponentList(softRigidBodies, (x) => { x.AddForce(direction * x.mass, ForceMode2D.Impulse); });
        }

        public void SwitchGravity(bool active) {
            SetActionToComponentList(softRigidBodies, (x) => {
                x.gravityScale = active ? 1 : 0;
            });
            animatedViewRbody.gravityScale = active ? 1 : 0;
        }

        public async Task AnimateSpawn() {
            body.gameObject.SetActive(true);
            trigger.gameObject.SetActive(true);
            animationViewCollider.gameObject.SetActive(true);
            await additionalRoot.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

            softBody.SetActive(true);
            animationViewCollider.gameObject.SetActive(false);
        }

        public async void AnimateDeath(Vector3 mergePosition) {
            IsMarkedToKill = true;
            softBody.SetActive(false);
            body.gameObject.SetActive(false);
            trigger.gameObject.SetActive(false);
            animationViewCollider.gameObject.SetActive(true);
            SwitchGravity(false);
            trailRenderer.emitting = false;

            for (var i = 0; i < softColliders.Length; i++) softColliders[i].gameObject.SetActive(true);
            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = RigidbodyConstraints2D.FreezeAll; });

            await DOTween.Sequence()
                .Append(additionalRoot.DOScale(0, 0.2f))
                .Join(additionalRoot.DOMove(mergePosition, 0.3f))
                .AsyncWaitForCompletion();
            
            Kill();
        }

        public void Kill() {
            animationViewCollider.gameObject.SetActive(false);
            OnDeath?.Invoke(this);
        }

        private void OnTriggerHandler(Collider2D collider) {
            if (IsMarkedToKill) return;

            OnTrigger?.Invoke(body, collider);
        }

        private void SetActionToComponentList<TComponent>(TComponent[] list, Action<TComponent> action) {
            for (var i = 0; i < list.Length; i++) {
                var entry = list[i];
                action.Invoke(entry);
            }
        }

        private Vector3 GetAverageVelocity(Rigidbody2D[] rigidBodies, bool draw = false) {
            var velocitySumm = Vector2.zero;
            for (var i = 0; i < rigidBodies.Length; i++) velocitySumm += rigidBodies[i].velocity;
            var result = velocitySumm * (1f / rigidBodies.Length);
            return result;
        }

        private void OnDrawGizmos() {
            if (softRigidBodies == null || softRigidBodies.Length == 0) return;

            var velocitySumm = Vector2.zero;
            for (var i = 0; i < softRigidBodies.Length; i++) {
                var rb = softRigidBodies[i];
                velocitySumm += rb.velocity;
                if (rb.velocity.magnitude > 0) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(rb.transform.position, rb.transform.position + new Vector3(rb.velocity.x, rb.velocity.y, 0));
                }
            }

            var result = velocitySumm * (1f / softRigidBodies.Length);
            if (result.magnitude > 0) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + new Vector3(result.x, result.y, 0));
            }
        }
    }
}
