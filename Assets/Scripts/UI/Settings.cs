using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Settings : MonoBehaviour
    {

        [SerializeField] private ToggleSlider fullScreen;
        
        [SerializeField] private Slider LookSlider;
        [SerializeField] private Slider MusicSlider;
        [SerializeField] private Slider SoundSlider;
        [SerializeField] private Slider UISlider;
        
        [SerializeField] private TextMeshProUGUI lookSliderText;
        [SerializeField] private TextMeshProUGUI musicSliderText;
        [SerializeField] private TextMeshProUGUI soundSliderText;
        [SerializeField] private TextMeshProUGUI uISliderText;
        
        [SerializeField] private TMP_Dropdown graphics;
        [SerializeField] private AudioClip openNoise;
        [SerializeField] private AudioClip closeNoise;
    
    
        private static Settings _settings;

        private bool _inMainMenu = true ;
        private bool _init;
        public static void OpenSettings(bool inMainMenu)
        {       
            _settings._inMainMenu = inMainMenu;
            _settings.gameObject.SetActive(true);
            GameManager.PlayUISound(_settings.openNoise);
        }

 

        private void OnEnable()
        {
        
            if(!_inMainMenu) PlayerControls.EnableUI();
        }

        private void Start()
        {

            if (_settings && _settings != this)
            {
                Destroy(gameObject);
                return;
            }
        
            _settings = this;
            DontDestroyOnLoad(gameObject);
        
            _settings.transform.GetChild(0).gameObject.SetActive(true);
        
            //graphics.value = QualitySettings.GetQualityLevel();

            
            //Apply Loaded Settings
            LookSlider.onValueChanged.AddListener(SetLookSensitivity);
            MusicSlider.onValueChanged.AddListener(SetMusicVolume);
            SoundSlider.onValueChanged.AddListener(SetSoundVolume);
            UISlider.onValueChanged.AddListener(SetUIScale);
            graphics.onValueChanged.AddListener(SetGraphicsQuality);
            fullScreen.OnValueChanged += SetFullScreen;
            
            Rebind();
        
            gameObject.SetActive(false);
            
            //If we're not playing on standalone, we don't need the button.
            #if !UNITY_STANDALONE
            fullScreen.gameObject.SetActive(false);
            #endif
            
        }
        
        private void SetFullScreen(bool val)
        {
            FullScreenMode x = val ? FullScreenMode.FullScreenWindow: FullScreenMode.Windowed;
            SettingsManager.currentSettings.fullScreen = x;
            Screen.fullScreenMode = x; // There is a boolean option, but this is a little bit more flexible for porting the code.
        }

        private void SetGraphicsQuality(int val)
        {
            
            SettingsManager.currentSettings.graphics = (SettingsManager.EGraphicsState)val;
            QualitySettings.SetQualityLevel(val);

        }

        public void ResetSaveData()
        {
            SettingsManager.ResetSettings();
            Rebind();
            SaveData();
        }

        private void Rebind()
        {

            
            LookSlider.SetValueWithoutNotify(SettingsManager.currentSettings.LookSensitivity);
            lookSliderText.text = SettingsManager.currentSettings.LookSensitivity.ToString("F");
        
            MusicSlider.SetValueWithoutNotify(SettingsManager.currentSettings.MusicVolume);
            musicSliderText.text = SettingsManager.currentSettings.MusicVolume.ToString("P0");

            SoundSlider.SetValueWithoutNotify(SettingsManager.currentSettings.SoundVolume);
            soundSliderText.text = SettingsManager.currentSettings.SoundVolume.ToString("P0");

            UISlider.SetValueWithoutNotify(SettingsManager.currentSettings.UIScale);
            uISliderText.text = SettingsManager.currentSettings.UIScale.ToString("P0");
        
            graphics.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
            graphics.RefreshShownValue();
            fullScreen.State = SettingsManager.currentSettings.fullScreen == FullScreenMode.FullScreenWindow;

        }

        private void SaveData()
        {
            SettingsManager.SaveData();
        }

        private void SetLookSensitivity(float val)
        {
            SettingsManager.currentSettings.LookSensitivity = val;
            lookSliderText.text = val.ToString("F");

        }

        private void SetSoundVolume(float val)
        {
            SettingsManager.currentSettings.SoundVolume = val;
            soundSliderText.text = val.ToString("P0");

        }

        private void SetMusicVolume(float val)
        {
            SettingsManager.currentSettings.MusicVolume = val;
            musicSliderText.text = val.ToString("P0");

        }

        private void SetUIScale(float val)
        {
            SettingsManager.currentSettings.UIScale = val;
            uISliderText.text = val.ToString("P0");
            print("SetUIScale set");

        }

        private void OnDisable()
        {
            if (!_init)
            {
                _init = true;
                return;
            }
            SaveData();
            if (_inMainMenu)
            {
                MainMenuCanvas.SetActive(true);
            }
            else
            {
                PlayerControls.DisableUI();
            }
            _inMainMenu = false;
        }

        public static void CloseSettings()
        {
            _settings.gameObject.SetActive(false);
            GameManager.PlayUISound(_settings.closeNoise);
        }
    }
}
