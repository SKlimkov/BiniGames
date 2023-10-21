using System;
using UnityEngine;

namespace BiniGames.Effects {
    public class CollideEffect : MonoBehaviour, IPoolable {
        [SerializeField] private ParticleSystem collideParticle;

        public event Action<CollideEffect> OnComplete;

        public void OnPool() { }

        public void OnPop() { }

        public void Play(Color color1, Color color2, Vector2 sizes) {
            var main = collideParticle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color1, color2);
            main.startSize = new ParticleSystem.MinMaxCurve(Math.Min(sizes.x, sizes.y) * 0.2f, Math.Max(sizes.x, sizes.y) * 0.2f);
            collideParticle.Play();
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped() {
            OnComplete?.Invoke(this);
        }
    }
}
