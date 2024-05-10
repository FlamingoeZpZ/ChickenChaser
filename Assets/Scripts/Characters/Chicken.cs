using System;
using Ability;
using Game;
using Interfaces;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Chicken : MonoBehaviour, IVisualDetectable
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float maxSpeed;

        [Header("Looking")] 
        [SerializeField , Range(0,90)] private float pitchLimit;
        [SerializeField, Range(0,180)] private float yawLimit;
        [SerializeField, Range(0.01f,5)] private float yawSpeed;
        [SerializeField, Range(0.01f,5)] private float pitchSpeed;
        [SerializeField] private Transform head;

        [Header("Foot Management")] 
        [SerializeField] protected Transform footTransform;
        [SerializeField] protected float footRadius;
        [SerializeField] protected float footDistance;

        [Header("Effects")] 
        [SerializeField] private ParticleSystem landEffect;
        [SerializeField] private AudioClip bounceSfx;
        [SerializeField] private AudioClip bushNoise;
        [SerializeField] private GameObject caughtCam;
        public bool IsGrounded { get; private set; }
        public Vector3 HeadForward => head.forward;

        private Vector3 _moveDirection;
        private Vector2 _lookDirection;

        private Rigidbody _rb;
        private AbilityBase _abilityBaseController;
        private Animator _animator;

        public AbilityBase Ability => _abilityBaseController;
        

        private float visibility = 1;

        //private static Transform _myTransform;
        //public static Vector3 PlayerPosition => _myTransform.position;

        private float fallTime;

        // Start is called before the first frame update
        void Awake()
        {
            //_myTransform = transform;
            
            
            _rb = GetComponent<Rigidbody>();
            _abilityBaseController = GetComponent<AbilityBase>();
            _animator = GetComponentInChildren<Animator>();
            ChickenAnimatorReciever car = transform.GetChild(0).GetComponent<ChickenAnimatorReciever>();
            car.OnLandEffect += LandEffect;
            
            
            float abilityID = _abilityBaseController.BindOwner(this);
            _animator.SetFloat(StaticUtilities.AbilityTypeAnimID, abilityID);
            PlayerControls.Init(this);
            PlayerControls.DisableUI();
            _rb.maxLinearVelocity = maxSpeed;
            
           HudManager.Instance.BindPlayer(this);
            
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
            _moveDirection = Vector3.zero;
            _lookDirection = Vector2.zero;

            caughtCam.SetActive(true);
            
            enabled = false;
        }

        private void OnGameWon()
        {
            print("Player regis won game");
           PlayerControls.EnableUI();
           //Stop camera movement
           //_moveDirection = Vector3.zero;
           _lookDirection = Vector2.zero;

           _moveDirection = transform.TransformDirection((transform.position - EndGoal.TargetPosition).normalized);
        }

        private void FixedUpdate()
        {
            HandleGroundState();
            HandleMovement();
        }

        private void LateUpdate()
        {
            _animator.SetFloat(StaticUtilities.MoveSpeedAnimID, _rb.velocity.magnitude);
            HandleLooking();
        }

        private void HandleLooking()
        {
            float timeShift = Time.deltaTime;
            float pitchChange = head.localEulerAngles.x - pitchSpeed * _lookDirection.y * timeShift;
            float yawChange = transform.localEulerAngles.y + yawSpeed * _lookDirection.x * timeShift;
        
            if (pitchChange > pitchLimit && pitchChange < 180) pitchChange = pitchLimit;
            else if (pitchChange < 360-pitchLimit && pitchChange > 180) pitchChange = -pitchLimit;
            if (yawChange > yawLimit && yawChange < 180) yawChange = yawLimit;
            else if (yawChange < 360-yawLimit && yawChange > 180) yawChange = -yawLimit;

            transform.localEulerAngles = new Vector3(0, yawChange, 0);
            head.localEulerAngles = new Vector3(pitchChange, 0, 0);
        }

        private void HandleGroundState()
        {
            //We're going to spherecast downwards, and detect if we've hit the floor.
            //Basic Spherecast check
            bool newGroundedState = Physics.SphereCast(footTransform.position, footRadius, Vector3.down, out RaycastHit _, footDistance,
                StaticUtilities.WallLayer);
            if (newGroundedState != IsGrounded)
            {
                IsGrounded = newGroundedState;
                //Then we should update our grounded state.
                _animator.SetBool(StaticUtilities.IsGroundedAnimID, IsGrounded);

                if (IsGrounded)
                {
                    LandEffect(Mathf.Max(fallTime / 2, 3));
                    fallTime = 0;
                }
            }

            if (!IsGrounded) fallTime += Time.deltaTime;
        }

        private void HandleMovement()
        {
            _rb.AddForce(transform.rotation * _moveDirection * speed);
            _animator.SetFloat(StaticUtilities.MoveSpeedAnimID, _rb.velocity.magnitude);
        }


        public void TryAbility()
        {
            if(_abilityBaseController.TryAbility())
                _animator.SetTrigger(StaticUtilities.AbilityAnimID);
        }

    
        public void Look(Vector2 readValue)
        {
            _lookDirection = readValue;
        }

        public void Move(Vector2 readValue)
        {
            _moveDirection = new Vector3(readValue.x, 0, readValue.y);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            GizmosExtras.DrawWireSphereCast(footTransform.position, Vector3.down, footDistance, footRadius);
        }

        private void LandEffect(float force)
        {
            landEffect.emission.SetBurst(0, new ParticleSystem.Burst(0, Random.Range(10,20) * force));
            landEffect.Play();
            AudioManager.Instance.PlaySound(bounceSfx, transform.position, 0.25f, 5 * force);
            
            //If in bush
            if(Physics.CheckSphere(transform.position, footRadius, StaticUtilities.BushLayer))
                AudioManager.Instance.PlaySound(bushNoise, transform.position, 0.25f, 15 * force);
            
        }

        public void AddVisibility(float vis)
        {
            visibility += vis;
        }

        public void RemoveVisibility(float vis)
        {
            //Prevent any weirdness
            visibility = Mathf.Max(visibility - vis, 0.1f);
        }

        public float GetVisibility()
        {
            return visibility;
        }
    }
}
