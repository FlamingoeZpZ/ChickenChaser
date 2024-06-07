using Managers;
using UnityEngine;

namespace UI
{
    public class MainMenuCanvas : MonoBehaviour
    {
        private static MainMenuCanvas _menuCanvas;
        [SerializeField] private GameObject quitButton;
        
        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            
        }

        private void Start()
        {
            _menuCanvas = this;
            #if UNITY_WEBGL
            quitButton.SetActive(false);
            #endif
        }

        public void OpenSettings()
        {
            Settings.OpenSettings(true);
        }

        public void OpenRP4K()
        {
            Application.OpenURL("https://realprogramming.com/");
        }

        public void BeginGame()
        {
            GameManager.LoadGame();
        }

        public static void SetActive(bool b)
        {
            if(_menuCanvas) _menuCanvas.gameObject.SetActive(b);
        }

        public void PlaySound(AudioClip hoverSound)
        {
            GameManager.PlayUISound(hoverSound);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
