using System;
using Managers;
using UnityEngine;

namespace UI
{
    public class MainMenuCanvas : MonoBehaviour
    {
        private static MainMenuCanvas _menuCanvas;
        
        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            
        }

        private void Start()
        {
            _menuCanvas = this;
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
            _menuCanvas.gameObject.SetActive(b);
        }

        public void PlaySound(AudioClip hoverSound)
        {
            GameManager.PlayUISound(hoverSound);
        }
    }
}
