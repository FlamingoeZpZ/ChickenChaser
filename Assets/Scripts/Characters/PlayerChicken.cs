using System;
using Ability;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

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
        [SerializeField] private AbilityBase jumpAbility;
        
        public AbilityBase Ability => abilityBaseController;
        public AbilityBase Cluck => cluckAbility;
        public AbilityBase JumpAbility => jumpAbility;

        public static Action<Vector3> onPlayerCaught;
        public static Action<Vector3> onPlayerEscaped;
        public static Action onPlayerRescued; // It's not out of the picture that another chicken can rescue the player.
        
        protected override void Awake()
        {
            base.Awake(); 
            HudManager.Instance.BindPlayer(this);
            PlayerControls.Init(this);
            PlayerControls.DisableUI();
            
            
        }

        public override void ReleaseChicken()
        {
            //The player chicken in this version of the game will always be trapped.
            //It's possible to make it so that the other chickens can free the player, and reenable it.
            
            enabled = true;
            PlayerControls.DisableUI();
            onPlayerRescued?.Invoke();
            caughtCam.SetActive(false);

            //Remove pop-up
            cluckAbility.StopAbility();
        }

        public override void EscapeAndMoveTo(Vector3 position)
        {
            print("Player WON game");
            
            //Stop Inputs
            PlayerControls.DisablePlayer();
            
            //Stop movement
            LookDirection = Vector2.zero;
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
                agent.enabled = true;
                agent.SetDestination(position);
            
            caughtCam.SetActive(false);
            
            //Tell the GameManager that we lost
            GameManager.PlayUISound(stats.OnEscape);
            
            onPlayerEscaped?.Invoke(transform.position);
            //Tell the UI that we lost
            
        }

        public override void CaptureChicken()
        {
            print("Player LOST game");
            
            //Stop the animator
            Animator.SetFloat(StaticUtilities.MoveSpeedAnimID, 0);
            cluckAbility.StartAbility();
            
            //Toggle Camera effects
            caughtCam.SetActive(true);
            
            //Disable all other controls
            enabled = false;
            
            //Tell the GameManager that we lost
            onPlayerCaught?.Invoke(transform.position);
            GameManager.PlayUISound(stats.OnCapture);
            

            //Tell the UI that we lost
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }


        private void OnEnable()
        {
            SettingsManager.SaveFile.onLookSenseChanged += OnLookSensChanged;
            _lookSpeed = SettingsManager.currentSettings.LookSensitivity;
            Rb.isKinematic = false;
            _collider.enabled = true;
        }
    
    
        private void OnDisable()
        {
            SettingsManager.SaveFile.onLookSenseChanged -= OnLookSensChanged;
            Rb.isKinematic = true;
            _collider.enabled = false;
            
            PlayerControls.DisablePlayer();

            abilityBaseController.ForceCancelAbility();
            cluckAbility.ForceCancelAbility();
            jumpAbility.ForceCancelAbility();
            
            Move(Vector2.zero);
            Look(Vector2.zero);
            
        }

        private void OnLookSensChanged(float obj)
        {
            _lookSpeed = obj;
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
        
        public void ChangeJumpState(bool state)
        {
            if (state) jumpAbility.StartAbility();
            else jumpAbility.StopAbility();
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
