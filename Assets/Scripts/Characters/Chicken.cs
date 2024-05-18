using System;
using Ability;
using Game;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] protected Transform head;

        [Header("Foot Management")] 
        [SerializeField] protected Transform footTransform;
        [SerializeField] protected float footRadius;
        [SerializeField] protected float footDistance;

        [Header("Effects")] 
        [SerializeField] private ParticleSystem landEffect;
        [SerializeField] private AudioClip bounceSfx;
        [SerializeField] private AudioClip bushNoise;

        public bool IsGrounded { get; private set; }
        public Vector3 HeadForward => head.forward;

        protected Vector3 MoveDirection;
        protected Vector2 LookDirection;

        private Rigidbody _rb;
        
        [SerializeField] private AbilityBase abilityBaseController;
        [SerializeField] private AbilityBase cluckAbility;
        
        private Animator _animator;

        public AbilityBase Ability => abilityBaseController;
        public AbilityBase Cluck => cluckAbility;
        
        private float _visibility = 1;

        //private static Transform _myTransform;
        //public static Vector3 PlayerPosition => _myTransform.position;

        private float _fallTime;

        // Start is called before the first frame update
        protected virtual void Awake()
        {
            //_myTransform = transform;
            
            
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            ChickenAnimatorReciever car = transform.GetChild(0).GetComponent<ChickenAnimatorReciever>();
            car.OnLandEffect += LandEffect;
            
            _rb.maxLinearVelocity = maxSpeed;
        }


        private void FixedUpdate()
        {
            HandleGroundState();
            HandleMovement();
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
                    LandEffect(Mathf.Max(_fallTime / 2, 3));
                    _fallTime = 0;
                }
            }

            if (!IsGrounded) _fallTime += Time.deltaTime;
        }

        private void HandleMovement()
        {
            _rb.AddForce(transform.rotation * MoveDirection * speed);
            _animator.SetFloat(StaticUtilities.MoveSpeedAnimID, _rb.velocity.magnitude);
        }


        public void ChangeAbilityState(bool state)
        {
            if(abilityBaseController.TryAbility()) 
                _animator.SetTrigger(abilityBaseController.AbilityNum());
        }


        public void ChangeCluckState(bool state)
        {
            if(cluckAbility.TryAbility()) 
                _animator.SetTrigger(cluckAbility.AbilityNum());
        }


        public void Look(Vector2 readValue)
        {
            LookDirection = readValue;
        }

        public void Move(Vector2 readValue)
        {
            MoveDirection = new Vector3(readValue.x, 0, readValue.y);
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
            _visibility += vis;
        }

        public void RemoveVisibility(float vis)
        {
            //Prevent any weirdness
            _visibility = Mathf.Max(_visibility - vis, 0.1f);
        }

        public float GetVisibility()
        {
            return _visibility;
        }
    }
}
