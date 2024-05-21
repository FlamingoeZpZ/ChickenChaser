using Game;
using Managers;
using UnityEngine;

namespace Characters
{
    public class PlayerChicken : Chicken
    {
        [Header("Looking")] 
        [SerializeField , Range(0,90)] private float pitchLimit;
        [SerializeField, Range(0,180)] private float yawLimit;
        [SerializeField, Range(0.01f,5)] private float yawSpeed;
        [SerializeField, Range(0.01f,5)] private float pitchSpeed;
    
        [Header("Effects")]
        [SerializeField] private GameObject caughtCam;
    
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
        }
    
    
        private void OnDisable()
        {
            print("Unbinding Events");

            EndGoal.onGameWon -= OnGameWon;
            CaptureZone.onGameLoss -= OnGameLoss;
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
    
        private void HandleLooking()
        {
            float timeShift = Time.deltaTime;
            float pitchChange = head.localEulerAngles.x - pitchSpeed * LookDirection.y * timeShift;
            float yawChange = transform.localEulerAngles.y + yawSpeed * LookDirection.x * timeShift;
        
            if (pitchChange > pitchLimit && pitchChange < 180) pitchChange = pitchLimit;
            else if (pitchChange < 360-pitchLimit && pitchChange > 180) pitchChange = -pitchLimit;
            if (yawChange > yawLimit && yawChange < 180) yawChange = yawLimit;
            else if (yawChange < 360-yawLimit && yawChange > 180) yawChange = -yawLimit;

            transform.localEulerAngles = new Vector3(0, yawChange, 0);
            head.localEulerAngles = new Vector3(pitchChange, 0, 0);
        }
    }
}
