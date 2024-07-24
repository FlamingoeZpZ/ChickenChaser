using System;
using System.Collections.Generic;
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
        [SerializeField] private AbilityUIBind abilityC;
        
        private PlayerChicken _owner;
        
        [Header("Chickens")]
        [SerializeField] private Image cagedObject;
        [SerializeField] private Image unCagedObject;
        [SerializeField] private Transform cagedTransform;
        [SerializeField] private Transform unCagedTransform;

        private readonly Dictionary<AIChicken, Tuple<GameObject, GameObject>> _hudChickens = new();
        
        
        private void Awake()
        {
            print("Hud Awake");
            if(Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
           

            
#if !UNITY_STANDALONE && !UNITY_WEBGL
            stick.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);
            settingsButton.onClick.AddListener(EnterSettings);
#endif
        }
        
        
        #region Registering Chickens
        public void BindPlayer(PlayerChicken player)
        {
            _owner = player;
            
            //Bind abilities
            abilityA.SetTargetAbility(player.Ability);
            abilityB.SetTargetAbility(player.Cluck);
            abilityC.SetTargetAbility(player.JumpAbility);
        }

        public void RegisterChicken(AIChicken chicken)
        {
            GameObject a = Instantiate(cagedObject, cagedTransform).gameObject;
            GameObject b = Instantiate(unCagedObject, unCagedTransform).gameObject;

            a.SetActive(false);
            
            chicken.OnCaught += () => OnChickenCaptured(chicken);
            chicken.OnFree += () => OnChickenRescued(chicken);
            
            _hudChickens.Add(chicken, new (a,b));
        }

        public void RemoveChicken(AIChicken chicken)
        {
            Destroy(_hudChickens[chicken].Item1);
            Destroy(_hudChickens[chicken].Item2);
            _hudChickens.Remove(chicken);
        }

        private void OnChickenRescued(AIChicken chicken)
        {
            _hudChickens[chicken].Item1.SetActive(true);
            _hudChickens[chicken].Item2.SetActive(false);
        }

        private void OnChickenCaptured(AIChicken chicken)
        {
            _hudChickens[chicken].Item1.gameObject.SetActive(false);
            _hudChickens[chicken].Item2.gameObject.SetActive(true);
        }


        #endregion
        
        #region Mobile
        
        //We want these variables to exist in the Unity editor so we can customize, but not to exist in builds that don't need them.
#if (!UNITY_STANDALONE && !UNITY_WEBGL) || UNITY_EDITOR
        [SerializeField] private Joystick stick;
        [SerializeField] private Button settingsButton;
        [SerializeField] private float lookSpeed = 20; // Just to make looking around a bit easier
        [SerializeField, Range(0,1)] private float stickDeadZoneY = 0.02f; // Just to make looking around a bit easier
        private float _lookMultiplier;
#endif
        
#if !UNITY_STANDALONE && !UNITY_WEBGL

        private static Vector2 _displayScale;
        
        private void OnEnable()
        {
            SettingsManager.SaveFile.onLookSenseChanged += OnLookSensChanged;
            OnLookSensChanged(SettingsManager.currentSettings.LookSensitivity);

        }

        private void OnDisable()
        {
            SettingsManager.SaveFile.onLookSenseChanged -= OnLookSensChanged;

        }

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
            Vector2 value = new Vector2(stick.Direction.x, Mathf.Abs(stick.Direction.y) > stickDeadZoneY?stick.Direction.y/((float)Screen.currentResolution.width/Screen.currentResolution.height): 0);
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
