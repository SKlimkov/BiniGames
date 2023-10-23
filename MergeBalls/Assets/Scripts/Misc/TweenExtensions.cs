using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;
using System;

namespace BiniGames {
    public static class TweenExtensions {
        public static TweenerCore<float, float, FloatOptions> DoTimer(this float startTime, float duration) {
            var result = DOTween.To(() => Time.time, x => x = Time.time - startTime, startTime + duration, duration);
            result.SetUpdate(true).SetEase(Ease.Linear);
            return result;
        }

        public static TweenerCore<float, float, FloatOptions> DoTimer(this float startTime, float duration, float startValue, float endValue, Action<float> action) {
            var timeCovered = Time.time - startTime;
            var result = DOTween.To(() => timeCovered, x => timeCovered = x, startTime + duration, duration).OnUpdate(() => { action?.Invoke(Mathf.Lerp(startValue, endValue, timeCovered / duration)); });
            result.SetUpdate(true).SetEase(Ease.Linear);
            return result;
        }
    }
}
