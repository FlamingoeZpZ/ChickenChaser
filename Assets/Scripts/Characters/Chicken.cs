using System;
using Ability;
using UnityEngine;
using Utilities;

namespace Characters
{
    [RequireComponent(typeof(AbilityBase))]
    public class Chicken : MonoBehaviour
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
    
        public bool IsGrounded { get; private set; }
        public Vector3 HeadForward => head.forward;

        private Vector3 _moveDirection;
        private Vector2 _lookDirection;

        private Rigidbody _rb;
        private AbilityBase _abilityBaseController;
        private Animator _animator;

        private static Transform _myTransform;
        public static Vector3 PlayerPosition => _myTransform.position;

        // Start is called before the first frame update
        void Awake()
        {
            _myTransform = transform;
            _rb = GetComponent<Rigidbody>();
            _abilityBaseController = GetComponent<AbilityBase>();
            _animator = GetComponentInChildren<Animator>();
            float abilityID = _abilityBaseController.BindOwner(this);
            _animator.SetFloat(StaticUtilities.AbilityTypeAnimID, abilityID);
            PlayerControls.Init(this);
            PlayerControls.DisableUI();
            _rb.maxLinearVelocity = maxSpeed;
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
            }
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
    }
}
