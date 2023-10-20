using UnityEngine;

namespace BiniGames.UI {
    public class UIManager : MonoBehaviour {
        [SerializeField] LoadingScreenWindow loadingScreenWindow;
        [SerializeField] VictoryScreenWindow winScreenWindow;

        private void Awake() {
            loadingScreenWindow.Show();
            winScreenWindow.Hide();
        }

        public void HideLoadingScreen() {
            loadingScreenWindow.Hide();
        }

        public void ShowWinScreen() {
            winScreenWindow.Hide();
            winScreenWindow.Show();
        }
    }
}
