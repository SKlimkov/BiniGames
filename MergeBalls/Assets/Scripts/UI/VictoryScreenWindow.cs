using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BiniGames.UI {
    public class VictoryScreenWindow : MonoBehaviour {
        [SerializeField] RectTransform winLabel;
        [SerializeField] Button okButton;
        [SerializeField] ParticleSystem particleSystem;

        private void Awake() {
            okButton.onClick.AddListener(OnOkButtonClick);
        }

        [EasyButtons.Button]
        public async void Show() {
            EventSystem.current.SetSelectedGameObject(null);
            gameObject.SetActive(true);
            particleSystem.Play();
            await winLabel.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

            okButton.gameObject.SetActive(true);
        }

        [EasyButtons.Button]
        public void Hide() {
            particleSystem.Stop();
            gameObject.SetActive(false);
            winLabel.localScale = Vector3.zero;
            okButton.gameObject.SetActive(false);
        }

        private void OnOkButtonClick() {
            Debug.LogErrorFormat("OnOkButtonClick");
        }
    }
}
