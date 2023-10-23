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
        [SerializeField] private CircleCollider2D trigger;
        [SerializeField] private Rigidbody2D triggerBody;
        [SerializeField] private Rigidbody2D rootRbody;
        [SerializeField] private Rigidbody2D animatedViewRbody;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Color color;
        [SerializeField] private GameObject softBody;
        [SerializeField] private Transform additionalRoot;
        [SerializeField] private TriggerEventWrapper triggerEventWrapper;
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color hideColor;
        
        private float[] softBodyRotations;
        private Vector2[] softBodyPositions;
        private Rigidbody2D[] softRigidBodies;
        private CircleCollider2D[] softColliders;

        public event Action<GameActor> OnDeath;
        public event Action<Collider2D, Collider2D> OnTrigger;

        public Material SharedMaterial { get { return spriteRenderers[0].sharedMaterial; } }
        public Vector3 Velocity { get { return GetAverageVelocity(softRigidBodies); } }
        public Color Color => color;
        public int Key => grade;
        public int ColliderId => trigger.GetInstanceID();
        public bool IsMarkedToKill;

        private void Awake() {
            softRigidBodies = softBody.GetComponentsInChildren<Rigidbody2D>();
            softBodyPositions = new Vector2[softRigidBodies.Length];
            softBodyRotations = new float[softRigidBodies.Length];
            for (var i = 0; i < softRigidBodies.Length; i++) {
                softBodyPositions[i] = softRigidBodies[i].transform.localPosition;
                softBodyRotations[i] = softRigidBodies[i].transform.localRotation.eulerAngles.z;
            }
            softColliders = softBody.GetComponentsInChildren<CircleCollider2D>();
            triggerEventWrapper.OnTriggerEnterEvent += OnTriggerEnterHandler;
            triggerEventWrapper.OnTriggerExitEvent += OnTriggerExitHandler;
            trigger.radius = animationViewCollider.radius;
            SetActionToComponentList(softRigidBodies, (x) => { x.mass = PhysicsHelpers.RadiusToMass(animationViewCollider.radius); });
            animatedViewRbody.mass = PhysicsHelpers.RadiusToMass(animationViewCollider.radius);
        }

        [EasyButtons.Button]
        private void ResetBody(bool activateOnComplete = false) {
            SetActionToComponentList(spriteRenderers, (x) => { x.color = hideColor; });
            SetActionToComponentList(softRigidBodies, (x) => { x.bodyType = RigidbodyType2D.Kinematic; });
            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = RigidbodyConstraints2D.None; });
            softBody.gameObject.SetActive(true);
            triggerBody.gameObject.SetActive(true);
            animatedViewRbody.gameObject.SetActive(true);

            rootRbody.bodyType = RigidbodyType2D.Kinematic;
            rootRbody.constraints = RigidbodyConstraints2D.None;

            triggerBody.bodyType = RigidbodyType2D.Kinematic;
            triggerBody.constraints = RigidbodyConstraints2D.None;

            animatedViewRbody.bodyType = RigidbodyType2D.Kinematic;
            animatedViewRbody.constraints = RigidbodyConstraints2D.None;

            for (var i = 0; i < softRigidBodies.Length; i++) {
                var rb = softRigidBodies[i];
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.transform.localPosition = softBodyPositions[i];
                rb.transform.localRotation = Quaternion.Euler(0, 0, softBodyRotations[i]);
            }

            rootRbody.velocity = Vector2.zero;
            rootRbody.angularVelocity = 0;
            rootRbody.transform.localRotation = Quaternion.identity;

            triggerBody.velocity = Vector2.zero;
            triggerBody.angularVelocity = 0;
            triggerBody.transform.localPosition = Vector3.zero;
            triggerBody.transform.localRotation = Quaternion.identity;

            animatedViewRbody.velocity = Vector2.zero;
            animatedViewRbody.angularVelocity = 0;
            animatedViewRbody.transform.localPosition = Vector3.zero;
            animatedViewRbody.transform.localRotation = Quaternion.identity;

            rootRbody.constraints = RigidbodyConstraints2D.None;
            rootRbody.bodyType = RigidbodyType2D.Dynamic;

            triggerBody.constraints = RigidbodyConstraints2D.None;
            triggerBody.bodyType = RigidbodyType2D.Dynamic;

            animatedViewRbody.constraints = RigidbodyConstraints2D.None;
            animatedViewRbody.bodyType = RigidbodyType2D.Dynamic;

            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = RigidbodyConstraints2D.FreezeAll; });
            SetActionToComponentList(softRigidBodies, (x) => { x.bodyType = RigidbodyType2D.Dynamic; });
            softBody.gameObject.SetActive(activateOnComplete);
            triggerBody.gameObject.SetActive(false);
            animatedViewRbody.gameObject.SetActive(false);
            SetActionToComponentList(spriteRenderers, (x) => { x.color = normalColor; });
        }

        public void OnPop() {
            ResetBody();

            SwitchGravity(false);
            IsMarkedToKill = false;
            trailRenderer.widthMultiplier = animationViewCollider.radius * 2f * 0.85f;            
            additionalRoot.localPosition = Vector3.zero;
        }

        public void OnPool() {
            transform.rotation = Quaternion.identity;
            SwitchGravity(false);
        }

        public void Fire(Vector3 direction) {
            SetActionToComponentList(softRigidBodies, x => { x.simulated = true; });
            SetActionToComponentList(softRigidBodies, (x) => { x.AddForce(direction * x.mass, ForceMode2D.Impulse); });

            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = RigidbodyConstraints2D.None; });
            animatedViewRbody.constraints = RigidbodyConstraints2D.None;
            rootRbody.constraints = RigidbodyConstraints2D.None;
        }

        public void SwitchGravity(bool active) {
            SetActionToComponentList(softRigidBodies, (x) => {
                x.gravityScale = active ? 1 : 0;
            });
            animatedViewRbody.gravityScale = active ? 1 : 0;
        }

        public async Task AnimateSpawn(bool isPlayer = false) {
            trailRenderer.Clear();
            trailRenderer.emitting = true;
            animationViewCollider.gameObject.SetActive(true);
            animatedViewRbody.gravityScale = 1;
            animatedViewRbody.constraints = isPlayer ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
            await additionalRoot.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

            SetActionToComponentList(softRigidBodies, x => { x.simulated = isPlayer ? false : true; });
            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = isPlayer ?  RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None; });
            rootRbody.constraints = isPlayer ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;

            trigger.gameObject.SetActive(true);
            softBody.SetActive(true);
            animatedViewRbody.gravityScale = 0;
            animationViewCollider.gameObject.SetActive(false);
        }

        public async void AnimateDeath(Vector3 mergePosition) {
            IsMarkedToKill = true;
            softBody.SetActive(false);
            trigger.gameObject.SetActive(false);
            animationViewCollider.gameObject.SetActive(true);
            animatedViewRbody.gravityScale = 1;
            SwitchGravity(false);
            trailRenderer.emitting = false;
            SetActionToComponentList(softRigidBodies, x => { x.simulated = false; });

            for (var i = 0; i < softColliders.Length; i++) softColliders[i].gameObject.SetActive(true);
            SetActionToComponentList(softRigidBodies, (x) => { x.constraints = RigidbodyConstraints2D.FreezeAll; });
            animatedViewRbody.constraints = RigidbodyConstraints2D.FreezeAll;
            rootRbody.constraints = RigidbodyConstraints2D.FreezeAll;

            await DOTween.Sequence()
                .Append(additionalRoot.DOScale(0, 0.2f))
                .Join(additionalRoot.DOMove(mergePosition, 0.3f))
                .AsyncWaitForCompletion();
            
            Kill();
        }

        public void Kill() {
            animationViewCollider.gameObject.SetActive(false);
            animatedViewRbody.gravityScale = 0;
            OnDeath?.Invoke(this);
        }

        //private int triggered;

        private void OnTriggerEnterHandler(Collider2D collider) {
            //triggered++;
            if (IsMarkedToKill) return;

            OnTrigger?.Invoke(trigger, collider);
        }

        private void OnTriggerExitHandler(Collider2D collider) {
            //triggered--;
            //if (triggered == 0) ResetBody(true);
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
