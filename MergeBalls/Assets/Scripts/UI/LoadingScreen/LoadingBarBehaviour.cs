using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BiniGames.UI {
    public class LoadingBarBehaviour : MonoBehaviour {
        [SerializeField] private Scrollbar scrollbar;
        private Sequence sequence;

        [EasyButtons.Button]
        public void StartTween() {
            scrollbar.value = 0;
            sequence = DOTween.Sequence()
                    .SetUpdate(UpdateType.Fixed, true)
                    .Append(DOTween.To(
                        () => scrollbar.value,
                        (x) => scrollbar.value = x,
                        1,
                        2f).SetEase(Ease.Linear))
                    .OnStepComplete(() => scrollbar.value = 0)
                    .SetLoops(-1);
        }

        [EasyButtons.Button]
        public void StopTween() {
            scrollbar.value = 0;
            sequence.Kill();
        }
    }
}
