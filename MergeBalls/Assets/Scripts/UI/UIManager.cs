using UnityEngine;

namespace BiniGames.UI {
    public class UIManager : MonoBehaviour {
        [SerializeField] LoadingScreenWindow loadingScreenWindow;

        private void Awake() {
            loadingScreenWindow.Show();
        }

        public void HideLoadingScreen() {
            loadingScreenWindow.Hide();
        }
    }
}
