using Ability;
using Game;
using Managers;
using UI;
using UnityEngine;

namespace Characters
{
    public class PlayerChicken : Chicken
    {
        [Header("Looking")] 
        [SerializeField , Range(0,90)] private float pitchLimit;
        [SerializeField, Range(0,180)] private float yawLimit;
        private float _lookSpeed;
    
        [Header("Effects")]
        [SerializeField] private GameObject caughtCam;
        [SerializeField] private AbilityBase abilityBaseController;
        [SerializeField] private AbilityBase cluckAbility;
        
        public AbilityBase Ability => abilityBaseController;
        public AbilityBase Cluck => cluckAbility;
        
        protected override void Awake()
        {
            base.Awake(); 
            HudManager.Instance.BindPlayer(this);
            PlayerControls.Init(this);
            PlayerControls.DisableUI();
        }
    
        private void OnEnable()
        {
            print("Binding Events");
            EndGoal.onGameWon += OnGameWon;
            CaptureZone.onGameLoss += OnGameLoss;
            SettingsManager.SaveFile.onLookSenseChanged += OnLookSensChanged;
            _lookSpeed = SettingsManager.currentSettings.LookSensitivity;
        }
    
    
        private void OnDisable()
        {
            print("Unbinding Events");

            EndGoal.onGameWon -= OnGameWon;
            CaptureZone.onGameLoss -= OnGameLoss;
            SettingsManager.SaveFile.onLookSenseChanged -= OnLookSensChanged;
        }

        private void OnLookSensChanged(float obj)
        {
            _lookSpeed = obj;
        }


        private void OnGameLoss()
        {
            PlayerControls.EnableUI();
            
            //Stop camera movement
            MoveDirection = Vector3.zero;
            LookDirection = Vector2.zero;

            caughtCam.SetActive(true);
            
            enabled = false;
        }

        private void OnGameWon()
        {
            print("Player regis won game");
            PlayerControls.EnableUI();
            //Stop camera movement
            //_moveDirection = Vector3.zero;
            LookDirection = Vector2.zero;

            MoveDirection = transform.TransformDirection((transform.position - EndGoal.TargetPosition).normalized);
        }
    
        private void LateUpdate()
        {
            HandleLooking();
        }
        
        
        public void ChangeAbilityState(bool state)
        {
            if(state) abilityBaseController.StartAbility();
            else abilityBaseController.StopAbility();
        }


        public void ChangeCluckState(bool state)
        {
            if (state) cluckAbility.StartAbility();
            else cluckAbility.StopAbility();
        }

    
        private void HandleLooking()
        {
            float timeShift = Time.deltaTime;
            float pitchChange = head.localEulerAngles.x - _lookSpeed * LookDirection.y * timeShift;
            float yawChange = transform.localEulerAngles.y + _lookSpeed * LookDirection.x * timeShift;
        
            if (pitchChange > pitchLimit && pitchChange < 180) pitchChange = pitchLimit;
            else if (pitchChange < 360-pitchLimit && pitchChange > 180) pitchChange = -pitchLimit;
            if (yawChange > yawLimit && yawChange < 180) yawChange = yawLimit;
            else if (yawChange < 360-yawLimit && yawChange > 180) yawChange = -yawLimit;

            transform.localEulerAngles = new Vector3(0, yawChange, 0);
            head.localEulerAngles = new Vector3(pitchChange, 0, 0);
        }
    }
}
