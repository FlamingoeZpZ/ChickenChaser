using Interfaces;
using ScriptableObjects;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Chicken : MonoBehaviour, IVisualDetectable
    {

        [SerializeField] private ChickenStats stats;
        
        [Header("Looking")] 
        [SerializeField] protected Transform head;

        [Header("Foot Management")] 
        [SerializeField] protected Transform footTransform;

        [Header("Effects")]
        [SerializeField] private ParticleSystem landEffect;
        


        public bool IsGrounded { get; private set; }
        public Vector3 HeadForward => head.forward;

        protected Vector3 MoveDirection;
        protected Vector2 LookDirection;
        protected Rigidbody Rb;
        protected Animator Animator;


        
        private float _visibility = 1;

        //private static Transform _myTransform;
        //public static Vector3 PlayerPosition => _myTransform.position;

        private float _fallTime;

        // Start is called before the first frame update
        protected virtual void Awake()
        {
            //_myTransform = transform;
            
            
            Rb = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();
            ChickenAnimatorReciever car = transform.GetChild(0).GetComponent<ChickenAnimatorReciever>();
            car.OnLandEffect += LandEffect;
            
            Rb.maxLinearVelocity = stats.MaxSpeed;
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
            bool newGroundedState = Physics.SphereCast(footTransform.position, stats.FootRadius, Vector3.down, out RaycastHit _, stats.FootDistance,
                StaticUtilities.WallLayer);
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
        }

        private void HandleMovement()
        {
            Rb.AddForce(transform.rotation * MoveDirection * stats.Speed);
            Animator.SetFloat(StaticUtilities.MoveSpeedAnimID, Rb.velocity.magnitude);
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
            AudioManager.Instance.PlaySound(stats.BounceSfx, transform.position, 0.25f, 5 * force);
            
            //If in bush
            if(Physics.CheckSphere(transform.position, stats.FootRadius, StaticUtilities.BushLayer))
                AudioManager.Instance.PlaySound(stats.BushSfx, transform.position, 0.25f, 15 * force);
            
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
