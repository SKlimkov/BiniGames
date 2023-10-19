using UnityEngine;

namespace BiniGames.UI {
    public class LoadingScreenWindow : MonoBehaviour {
        [SerializeField] LoadingBarBehaviour loadingBarBehaviour;

        private void Awake() {
            Show();
        }

        public void Show() {
            gameObject.SetActive(true);
            loadingBarBehaviour.StartTween();
        }

        public void Hide() {
            gameObject.SetActive(false);
            loadingBarBehaviour.StopTween();
        }
    }
}
