using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BiniGames.UI {
    public class VictoryScreenWindow : MonoBehaviour {
        [SerializeField] RectTransform winLabel;
        [SerializeField] Button nextButton;
        [SerializeField] ParticleSystem victoryParticles;

        private void Awake() {
            nextButton.onClick.AddListener(OnNextButtonClick);
        }

        [EasyButtons.Button]
        public async void Show() {
            EventSystem.current.SetSelectedGameObject(null);
            gameObject.SetActive(true);
            victoryParticles.Play();
            await winLabel.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

            nextButton.gameObject.SetActive(true);
        }

        [EasyButtons.Button]
        public void Hide() {
            victoryParticles.Stop();
            gameObject.SetActive(false);
            winLabel.localScale = Vector3.zero;
            nextButton.gameObject.SetActive(false);
        }

        private void OnNextButtonClick() {
            SceneManager.LoadScene(0);
        }
    }
}
