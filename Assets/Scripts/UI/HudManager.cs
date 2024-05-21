using Characters;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DefaultExecutionOrder(-1000)]
    public class HudManager : MonoBehaviour
    {
        public static HudManager Instance { get; private set; }
    
        //This should actually be in it's own file called "AbilityUI",
        //which binds directly to an ability and provides RO access
        [SerializeField] private Button abilityButton;
        [SerializeField] private Image abilityFillBar;
        [SerializeField] private Image abilityIcon;
        
        [SerializeField] private Button cluckButton;
        [SerializeField] private Image cluckFillBar;
        [SerializeField] private Image cluckIcon;
        
        [SerializeField] private Joystick stick;

        private PlayerChicken _owner;
        private Canvas _canvas;
        
        private void Awake()
        {
            if(Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _canvas = GetComponent<Canvas>();
            Instance = this;
        
#if !UNITY_STANDALONE || UNITY_EDITOR
            stick.gameObject.SetActive(true);
#endif
        
        }

        private void Start()
        {
            OnUIScaleChanged(SettingsManager.currentSettings.UIScale);
        }

        private void OnEnable()
        {
            SettingsManager.SaveFile.onUIScaleChanged += OnUIScaleChanged;
#if !UNITY_STANDALONE || UNITY_EDITOR
            SettingsManager.SaveFile.onLookSenseChanged += OnLookSensChanged;
            _lookMulitplier = SettingsManager.currentSettings.LookSensitivity;
            #endif
        }
#if !UNITY_STANDALONE || UNITY_EDITOR
        private void OnLookSensChanged(float obj)
        {
            _lookMulitplier = obj;
        }
#endif

        private void OnDisable()
        {
            SettingsManager.SaveFile.onUIScaleChanged -= OnUIScaleChanged;
#if !UNITY_STANDALONE || UNITY_EDITOR
            SettingsManager.SaveFile.onLookSenseChanged -= OnLookSensChanged;
            #endif
        }
    
        private void OnUIScaleChanged(float obj)
        {
            _canvas.scaleFactor = obj;// * 100;
        }

        public void BindPlayer(PlayerChicken player)
        {
            _owner = player;
            SetAbilityIcons();
            SetAbilityPercent(1);
            SetCluckAbilityPercent(1);
        
            _owner.Ability.BindAbilityUpdated(SetAbilityPercent);
            _owner.Cluck.BindAbilityUpdated(SetCluckAbilityPercent);

        }

        public void SetAbilityIcons()
        {
            abilityIcon.sprite = _owner.Ability.Icon;
            cluckIcon.sprite = _owner.Cluck.Icon;
        }

        public void SetAbilityPercent(float percent)
        {
            abilityFillBar.fillAmount = percent;
            abilityButton.interactable = percent >= 1;
        }
    
        public void SetCluckAbilityPercent(float percent)
        {
            cluckFillBar.fillAmount = percent;
            cluckButton.interactable = percent >= 1;
        }
    
        //Bind actions the same way our controller binds actions
        public void MimicActionAbility()
        {
            _owner.ChangeAbilityState(true);
        }

        public void MimicReleasedActionAbility()
        {
            _owner.ChangeAbilityState(false);
        }

        public void MimicCluckAbility()
        {
            _owner.ChangeCluckState(true);
        }
    
        public void MimicReleasedCluckAbility()
        {
            _owner.ChangeCluckState(false);
        }

#if !UNITY_STANDALONE || UNITY_EDITOR
        [SerializeField] private float lookSpeed = 5;
        private float _lookMulitplier;
#if UNITY_EDITOR
        [SerializeField] private bool stickEnabled;
#endif
        private void Update()
        {
#if UNITY_EDITOR
            if (!stickEnabled) return;
#endif

            _owner.Move(stick.Direction != Vector2.zero ? Vector2.up : Vector2.zero);
            _owner.Look(stick.Direction * (lookSpeed * _lookMulitplier));
        }
#endif
    }
}
