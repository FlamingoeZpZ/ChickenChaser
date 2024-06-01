using System;
using System.Collections;
using System.Globalization;
using Characters;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DefaultExecutionOrder(-1000)]
    public class HudManager : MonoBehaviour
    {
        public static HudManager Instance { get; private set; }
    
        [Header("HUDs")]
        [SerializeField] private Canvas hudCanvas;
        [SerializeField] private Canvas endCanvas;
        
        [Header("Interactables")]
        [SerializeField] private AbilityUIBind abilityA;
        [SerializeField] private AbilityUIBind abilityB;
        
        //We want these variables to exist in the Unity editor so we can customize, but not to exist in builds that don't need them.
#if (!UNITY_STANDALONE && !UNITY_WEBGL) || UNITY_EDITOR
        [SerializeField] private Joystick stick;
        [SerializeField] private Button settingsButton;
        [SerializeField] private float lookSpeed = 20; // Just to make looking around a bit easier
        [SerializeField, Range(0,1)] private float stickDeadZoneY = 0.02f; // Just to make looking around a bit easier
        private float _lookMultiplier;
#endif
        
        private PlayerChicken _owner;


        [Header("Chickens")]
        [SerializeField] private Sprite unCagedSprite;
        [SerializeField] private Sprite cagedSprite;
        [SerializeField] private Image templateObject;
        [SerializeField] private Transform cagedTransform;
        [SerializeField] private Transform unCagedTransform;

        [Header("End Canvas")] 
        [SerializeField] private TextMeshProUGUI endStatus;
        [SerializeField] private TextMeshProUGUI numChickensSaved;
        [SerializeField] private TextMeshProUGUI timeSpent;
        [SerializeField] private TextMeshProUGUI finalScore;
        [SerializeField] private GameObject hopeIsNotLost;
        [SerializeField] private Button mainMenu;
        
        //This should actually be it's own script.
        [Header("Score")] 
        [SerializeField] private AnimationCurve scoreCurve; // 0 is Quick, 1 is very long
        [SerializeField] private float expectedEndTime;
        [SerializeField] private int maximumTimePoints = 10000;
        [SerializeField] private int pointsPerSavedChicken = 1000;
        private float _cachedTime;
        private bool _canLoadMenu;
        private bool _cachedDidWin;
        
        
        private void Awake()
        {
            print("Hud Awake");
            if(Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
           
            hudCanvas.gameObject.SetActive(true);
            endCanvas.gameObject.SetActive(false);
            
            OnUIScaleChanged(SettingsManager.currentSettings.UIScale);
            
#if !UNITY_STANDALONE && !UNITY_WEBGL
            stick.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);
            settingsButton.onClick.AddListener(EnterSettings);
#endif
        }
        private void OnEnable()
        {
            SettingsManager.SaveFile.onUIScaleChanged += OnUIScaleChanged;
#if !UNITY_STANDALONE && !UNITY_WEBGL
            SettingsManager.SaveFile.onLookSenseChanged += OnLookSensChanged;
            OnLookSensChanged(SettingsManager.currentSettings.LookSensitivity);
#endif

            PlayerChicken.onPlayerCaught += LoseGame;
            PlayerChicken.onPlayerEscaped += WinGame;
            PlayerChicken.onPlayerRescued += OnPlayerRescued;

        }

        private void OnDisable()
        {
            SettingsManager.SaveFile.onUIScaleChanged -= OnUIScaleChanged;
#if !UNITY_STANDALONE && !UNITY_WEBGL
            SettingsManager.SaveFile.onLookSenseChanged -= OnLookSensChanged;
#endif
            
            PlayerChicken.onPlayerCaught -= LoseGame;
            PlayerChicken.onPlayerEscaped -= WinGame;
            PlayerChicken.onPlayerRescued -= OnPlayerRescued;
        }
    
        private void OnUIScaleChanged(float obj)
        {
            endCanvas.scaleFactor = obj;
            hudCanvas.scaleFactor = obj;
        }

        #region EndGame

        private void OnPlayerRescued()
        {
            _canLoadMenu = false;
            StopAllCoroutines();
            hudCanvas.gameObject.SetActive(true);
            endCanvas.gameObject.SetActive(false);
        }

        private void WinGame(Vector3 _) => OnBeginEndGame(true);
        private void LoseGame(Vector3 _) => OnBeginEndGame(false);
        
        private void OnBeginEndGame(bool won)
        {
            _canLoadMenu = true;
            StartCoroutine(EndGameTimer());
            hudCanvas.gameObject.SetActive(false);
            endCanvas.gameObject.SetActive(true);

            endStatus.text = won ? "ESCAPED" : "CAUGHT";
            //This is currently not recieving updates when a regular chicken escapes...
            _cachedTime = GameManager.TimeInLevel;
            _cachedDidWin = won;
            UpdateScore();
            //We need a method to tell whether we won.
        }

        private void UpdateScore()
        {
            numChickensSaved.text = GameManager.NumChickensSaved + "/" + GameManager.NumChickens;
            TimeSpan s = TimeSpan.FromSeconds(_cachedTime);
            timeSpent.text = $"{s.Minutes}m {s.Seconds}s {s.Milliseconds}ms";
            finalScore.text = ((_cachedDidWin?1 - scoreCurve.Evaluate(_cachedTime / expectedEndTime):0) * maximumTimePoints + (pointsPerSavedChicken * GameManager.NumChickensSaved)).ToString(CultureInfo.InvariantCulture);

            bool x = AIChicken.NumActiveAIChickens == 0;
            print("There are : " + AIChicken.NumActiveAIChickens +" Active chickens");
            //Determine which button to show
            hopeIsNotLost.SetActive(!x);
            mainMenu.gameObject.SetActive(x);
            
            mainMenu.Select();
        }

        public void LoadMainMenu()
        {
            //Only allow this to run once.
            if (!_canLoadMenu) return;
            GameManager.LoadMainMenu();
            _canLoadMenu = false;
        }

        //Simple timer
        private IEnumerator EndGameTimer()
        {
            yield return new WaitForSeconds(90); 
            LoadMainMenu();
        }

        #endregion
        
        #region Registering Chickens
        public void BindPlayer(PlayerChicken player)
        {
            _owner = player;
            
            //Bind abilities
            abilityA.SetTargetAbility(player.Ability);
            abilityB.SetTargetAbility(player.Cluck);
        }

        

        public void RegisterChicken()
        {
            print("Registering Chicken");
            Instantiate(templateObject, cagedTransform).sprite = cagedSprite;
        }

        public void OnChickenRescued()
        {
            print("Chicken Rescued");
            Transform t = cagedTransform.GetChild(0);
            t.SetParent(unCagedTransform ,false) ;
            t.GetComponent<Image>().sprite = unCagedSprite;
        }

        public void OnChickenCaptured()
        {
            print("Chicken Captured");
            Transform t = unCagedTransform.GetChild(0);
            t.SetParent(cagedTransform ,false) ;
            t.GetComponent<Image>().sprite = cagedSprite;
            UpdateScore();
        }

        public void OnChickenEscaped()
        {
            print("Chicken Escaped");
            Destroy(unCagedTransform.GetChild(0).gameObject);
            UpdateScore();
        }
        #endregion
        
        #region Mobile
#if !UNITY_STANDALONE && !UNITY_WEBGL

        private static Vector2 _displayScale;
        
        #if UNITY_EDITOR
        [SerializeField] private bool editorPhoneMode = true;
        #endif
        
        private void Update()
        {
            #if UNITY_EDITOR
            if (!editorPhoneMode) return;
            #endif
            _owner.Move(stick.Direction != Vector2.zero ? Vector2.up : Vector2.zero);
            //For looking, we should check to see if the magnitude is some really small number, if it is, we should actually just ignore it.
            Vector2 value = new Vector2(stick.Direction.x, Mathf.Abs(stick.Direction.y) > stickDeadZoneY?stick.Direction.y: 0);
            _owner.Look(Vector2.Scale(value ,_displayScale));
        }
        
        private void OnLookSensChanged(float obj)
        {
            _lookMultiplier = obj;
            _displayScale = new Vector2(1, (float)Screen.width / Screen.height) *(lookSpeed * _lookMultiplier);
        }
        
        private void EnterSettings()
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(ExitSettings);
            Settings.OpenSettings(false);
        }

        private void ExitSettings()
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(EnterSettings);
            Settings.CloseSettings();
        }
#endif
        #endregion
    }
}
