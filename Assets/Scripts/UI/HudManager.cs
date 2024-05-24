using Characters;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DefaultExecutionOrder(-1000)]
    public class HudManager : MonoBehaviour
    {
        public static HudManager Instance { get; private set; }
    
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
        private PlayerChicken _owner;
#endif
        
        [Header("HUD")]
        [SerializeField] private Canvas canvas;

        [Header("HUD | Chickens")]
        [SerializeField] private Sprite unCagedSprite;
        [SerializeField] private Sprite cagedSprite;
        [SerializeField] private Image templateObject;
        [SerializeField] private Transform cagedTransform;
        [SerializeField] private Transform unCagedTransform;

        private void Awake()
        {
            if(Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
           
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
        }

        private void OnDisable()
        {
            SettingsManager.SaveFile.onUIScaleChanged -= OnUIScaleChanged;
#if !UNITY_STANDALONE && !UNITY_WEBGL
            SettingsManager.SaveFile.onLookSenseChanged -= OnLookSensChanged;
#endif
        }
    
        private void OnUIScaleChanged(float obj)
        {
            canvas.scaleFactor = obj;
        }
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
            Instantiate(templateObject, unCagedTransform).sprite = cagedSprite;
        }

        public void OnChickenRescued()
        {
            print("Chicken Rescued");
            Transform t = unCagedTransform.GetChild(0);
            t.SetParent(cagedTransform ,false) ;
            t.GetComponent<Image>().sprite = unCagedSprite;
        }

        public void OnChickenCaptured()
        {
            print("Chicken Captured");
            Transform t = cagedTransform.GetChild(0);
            t.SetParent(unCagedTransform ,false) ;
            t.GetComponent<Image>().sprite = cagedSprite;
        }

        public void OnChickenEscaped()
        {
            print("Chicken Escaped");
            Destroy(cagedTransform.GetChild(0).gameObject);
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
