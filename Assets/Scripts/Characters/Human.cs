using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Characters
{
    public class Human : MonoBehaviour
    {
        private enum EHumanState
        {
            Idle, // From idle, after X seconds, we begin pathing. If we see the chicken, enter looking state.
            Pathing, // From pathing, we follow the track until we reach our destination. If we reach the destination, enter idle for X seconds. If we see the chicken, enter looking state
            Looking, // Do not move, if line of sight remains for Y seconds, begin chasing, else enter pathing
            Chasing // Chase the player, if line of sight is lost, enter the looking state.
        }

        [Header("AI")] [SerializeField] private Transform head;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private int currentPatrolPoint;

        [SerializeField, Min(0)] private float minIdleTime = 1;
        [SerializeField, Min(0)] private float maxIdleTime = 3;
        
        [Header("AI|Decay")]
        [SerializeField] private float maxDetection = 100;
        [SerializeField] private float detectionDecayRate = 5;
        [SerializeField] private float beginDecayCooldown = 3;
 
        [Header("AI | Vision Cone")] [SerializeField, Range(-1, 1)]
        private float visionFOV;

        [SerializeField, Min(0)] private float visionDistance;
        [SerializeField, Min(0)] private float sightDetectionValue = 2;
        [SerializeField] private AnimationCurve sightDetectionDropOff;


        [Header("AI | Audio")] [SerializeField, Min(0)]
        private float audioRange;

        [SerializeField, Min(0)] private float audioDetectionValue = 1;
        [SerializeField] private AnimationCurve audioDetectionDropOff;

        [Header("In Game")] [SerializeField] private MeshRenderer detectionBar;
        private Material _detectionBarMat;

        private float _currentDetection;
        private EHumanState _myState;
        private bool _canDetectionDecay = true;
        private bool _isDetectionDecaying;

        
        private Animator _animator;
        private NavMeshAgent _agent;

        private Coroutine _delayState;
        private Coroutine _decayTimer;

        public float CurrentDetection
        {
            get => _currentDetection;
            private set
            {
                _currentDetection = Mathf.Clamp(value, 0, maxDetection);
                float detectPerc = _currentDetection / maxDetection;
                _detectionBarMat.SetFloat(StaticUtilities.FillMatID, detectPerc);
                print("Detection State Change: " + detectPerc);
                if (_myState != EHumanState.Chasing && _myState != EHumanState.Looking)
                {
                    //Enter looking state.
                }
                else if (_myState ==  EHumanState.Looking && detectPerc >= 1)
                {
                    //Begin Chase.
                    print("I'm comin for you!");
                }
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            
            _detectionBarMat = detectionBar.material;
            _animator = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            AudioManager.onSoundPlayed += CheckListen;
            currentPatrolPoint -= 1;
            SetNextPatrolPoint();
            _myState = EHumanState.Pathing;
            CurrentDetection = 0;
        }

        private void CheckListen(Vector3 location, float volume)
        {
            //Get the distance between us and the location
            float distance = Vector3.Distance(head.position, location);
            
            //Sample the curve, based on the distance. (1 is close, and 0 is far.)

            float percentDistance =  1 - (distance / audioRange);

            CurrentDetection += percentDistance * audioDetectionValue;

            if (!_canDetectionDecay) return;
            if(_decayTimer != null) StopCoroutine(_decayTimer);
            _decayTimer = StartCoroutine(BeginDecayCooldown());
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetFloat(StaticUtilities.MoveSpeedAnimID, _agent.velocity.magnitude);
            LookDetection();
            
            if (_isDetectionDecaying)
            {
                CurrentDetection -= detectionDecayRate * Time.deltaTime;
            }

            if (_agent.remainingDistance <= _agent.stoppingDistance && _myState == EHumanState.Pathing)
            //if (_agent.isStopped && _myState == EHumanState.Pathing)
            {
                EnterIdle();
            }
        }

        void SetNextPatrolPoint()
        {
            _agent.SetDestination(patrolPoints[++currentPatrolPoint % patrolPoints.Length].position);
        }


        void EnterIdle()
        {
            _myState = EHumanState.Idle;
            _delayState = StartCoroutine(EnterStateAfterDelay(Random.Range(minIdleTime, maxIdleTime), EHumanState.Pathing));
        }

        void LookDetection()
        {
            //We need to KNOW where every target is...
            //Take the dot product from our direction to the player position...

            Vector3 playerLookDirection = (Chicken.PlayerPosition - head.position);
            float distance = playerLookDirection.magnitude;
            Vector3 normal = playerLookDirection/distance;
            float dot = Vector3.Dot(head.forward, normal);

            if (dot > visionFOV && distance < visionDistance)
            {
                print("I can see you");
                
                if (!Physics.Raycast(head.position, normal, out RaycastHit hit, distance, StaticUtilities.EverythingButChicken))
                {
                    print("I really can see you ya know.");
                    
                    //Compute the distance stuff...
                    float distancePerc = 1 - (distance / visionDistance);
                    CurrentDetection += sightDetectionDropOff.Evaluate(distancePerc) * sightDetectionValue; 
                    if(!_canDetectionDecay) return;
                    if(_decayTimer != null) StopCoroutine(_decayTimer);
                    _decayTimer = StartCoroutine(BeginDecayCooldown());
                }
            }

        }


        private IEnumerator EnterStateAfterDelay(float duration, EHumanState newState)
        {
            yield return new WaitForSeconds(duration);
            SetNextPatrolPoint(); // Cringe....
            _myState = newState;
        }


        private IEnumerator BeginDecayCooldown()
        {
            _canDetectionDecay = false;
            _isDetectionDecaying = false;
            yield return new WaitForSeconds(beginDecayCooldown);
            _isDetectionDecaying = true;
            _canDetectionDecay = true;
        }
        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
        
            for (int i = 0; i < patrolPoints.Length; ++i)
            {
                Gizmos.DrawSphere(patrolPoints[i].position, 0.25f);
                if (i == 0)
                {
                    Gizmos.DrawLine(patrolPoints[0].position, patrolPoints[^1].position);
                    continue;
                }

                Gizmos.DrawLine(patrolPoints[i-1].position, patrolPoints[i].position);
            }
        
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(head.position, audioRange);
            Gizmos.DrawRay(head.position, head.forward * visionDistance);
        
            //Dot product -- 1 in front, 0 sides, -1 behind.
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(visionFOV) * Mathf.Rad2Deg , head.right) * head.forward * visionDistance);
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(visionFOV) * Mathf.Rad2Deg , -head.right) * head.forward * visionDistance);
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(visionFOV) * Mathf.Rad2Deg , head.up) * head.forward * visionDistance);
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(visionFOV) * Mathf.Rad2Deg , -head.up) * head.forward * visionDistance);

            if (!Application.isPlaying) return; // Prevent Errors 
            Gizmos.color = Color.red;
            Vector3 d = Chicken.PlayerPosition - transform.position;
            Gizmos.DrawRay(transform.position, d);
        }
    }
}
