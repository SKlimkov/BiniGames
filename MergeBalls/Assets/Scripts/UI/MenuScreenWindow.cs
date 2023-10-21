using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BiniGames.UI {
    public class MenuScreenWindow : MonoBehaviour {
        [SerializeField] Button startButton;

        private void Awake() {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        private void OnStartButtonClick() {
            SceneManager.LoadScene(1);
        }
    }
}
