using AI;
using Interfaces;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Characters
{
    public abstract class Chicken : MonoBehaviour, IVisualDetectable
    {

        [SerializeField] protected ChickenStats stats;
        
        [Header("Looking")] 
        [SerializeField] protected Transform head;

        [Header("Foot Management")] 
        [SerializeField] protected Transform footTransform;

        [Header("Effects")]
        [SerializeField] private ParticleSystem landEffect;


        protected float moveSpeed;
        public bool IsGrounded { get; private set; }
        public Vector3 HeadForward => head.forward;
        public float MoveSpeed => moveSpeed;

        protected Vector3 MoveDirection;
        protected Vector2 LookDirection;
        protected Rigidbody Rb;
        protected Animator Animator;
        protected AudioSource audioSource;
        protected Collider _collider;
        
        private float _visibility = 1;

        private Vector3 slopeNormal;
        //private static Transform _myTransform;
        //public static Vector3 PlayerPosition => _myTransform.position;

        private float _fallTime;

        protected virtual void Awake()
        {
            //_myTransform = transform;

            audioSource = GetComponentInChildren<AudioSource>();
            Rb = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();
            _collider = GetComponentInChildren<Collider>();
            
            ChickenAnimatorReceiver car = transform.GetChild(0).GetComponent<ChickenAnimatorReceiver>();
            car.OnLandEffect += LandEffect;
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
            bool newGroundedState = Physics.SphereCast(footTransform.position, stats.FootRadius, Vector3.down, out RaycastHit slope, stats.FootDistance,
                StaticUtilities.GroundLayers);
            if (newGroundedState != IsGrounded)
            {
                IsGrounded = newGroundedState;
                //Then we should update our grounded state.
                Animator.SetBool(StaticUtilities.IsGroundedAnimID, IsGrounded);

                if (IsGrounded)
                {
                    LandEffect(Mathf.Max(_fallTime / 2, 3));
                    _fallTime = 0;
                    
                }

                
            }

            if (!IsGrounded) _fallTime += Time.deltaTime;
            else slopeNormal = slope.normal;
        }

        protected virtual void HandleMovement()
        {
            Vector3 direction = MoveDirection;
            if (IsGrounded)
            {
                direction = Vector3.ProjectOnPlane(MoveDirection, slopeNormal);
            }
            
            Rb.AddForce(transform.rotation * direction * stats.Speed, ForceMode.Acceleration);

            Vector2 horizontalVelocity = new Vector2(Rb.velocity.x, Rb.velocity.z);
            moveSpeed = horizontalVelocity.magnitude;

            if (moveSpeed > stats.MaxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * stats.MaxSpeed;
                Rb.velocity = new Vector3(horizontalVelocity.x, Rb.velocity.y, horizontalVelocity.y);
                moveSpeed = stats.MaxSpeed;
            }


            Animator.SetFloat(StaticUtilities.MoveSpeedAnimID, moveSpeed);
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
            GizmosExtras.DrawWireSphereCast(footTransform.position, Vector3.down, stats.FootDistance, stats.FootRadius);
        }

        private void LandEffect(float force)
        {
            landEffect.emission.SetBurst(0, new ParticleSystem.Burst(0, Random.Range(10,20) * force));
            landEffect.Play();
            
            //If we missed, we can't possibly find a clip...
            Vector3 pos = footTransform.position;
            
            //Make sure hit is not null
            if (!Physics.SphereCast(pos, stats.FootRadius, Vector3.down, out RaycastHit hit, stats.FootDistance,StaticUtilities.GroundLayers)) return;
           

            
            //Make sure the layer is not null
            if (!GameManager.SoundsDictionary.TryGetValue(hit.transform.tag, out AudioVolumeRangeSet set)) return;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            //Play the desired audio + detection
            audioSource.PlayOneShot(set.clip, set.volume);
            AudioDetection.onSoundPlayed.Invoke(pos, set.volume, set.rangeMultiplier * force, EAudioLayer.Chicken);
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

        public abstract void ReleaseChicken();

        public abstract void EscapeAndMoveTo(Vector3 position);
        public abstract void CaptureChicken();
    }
}
