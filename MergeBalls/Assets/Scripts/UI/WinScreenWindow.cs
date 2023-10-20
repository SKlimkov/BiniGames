using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BiniGames.UI {
    public class WinScreenWindow : MonoBehaviour {
        [SerializeField] RectTransform winLabel;
        [SerializeField] Button okButton;

        [EasyButtons.Button]
        public async void Show() {
            gameObject.SetActive(true);
            await winLabel.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
            okButton.enabled = true;
        }

        [EasyButtons.Button]
        public void Hide() {
            gameObject.SetActive(false);
            winLabel.localScale = Vector3.zero;
            okButton.enabled = false;
        }
    }
}
