using System.Collections;
using AI;
using Interfaces;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Characters
{
    public class Human : MonoBehaviour, IDetector
    {
        private enum EHumanState
        {
            Idle, // From idle, after X seconds, we begin pathing. If we see the chicken, enter looking state.
            Pathing, // From pathing, we follow the track until we reach our destination. If we reach the destination, enter idle for X seconds. If we see the chicken, enter looking state
            Looking, // Do not move, if line of sight remains for Y seconds, begin chasing, else enter pathing
            Chasing // Chase the player, if line of sight is lost, enter the looking state.
        }
        
        [SerializeField] private AiStats stats;

        [Header("In Game")] 
        [SerializeField] private MeshRenderer detectionBar;
        private Material _detectionBarMat;

        private float _currentDetection;
        private EHumanState _myState;
        private bool _isDetectionDecaying;
        private float _detectionModifier;

        private static readonly WaitForSeconds TimerDelay = new WaitForSeconds(0.016f);
        
        private Animator _animator;
        private PathHandler _pathHandler;
        private NavMeshAgent _agent;

        private Coroutine _decayTimer;
        private Coroutine _currentRoutine;

        private void Awake()
        {
            _detectionBarMat = detectionBar.material;
            _animator = GetComponentInChildren<Animator>();
            _pathHandler = GetComponent<PathHandler>();
            _agent = GetComponent<NavMeshAgent>();
            
            _myState = EHumanState.Pathing;
            _agent.speed = stats.BaseMoveSpeed;
            
            _currentDetection = 0;
            _detectionBarMat.SetFloat(StaticUtilities.FillMatID, 0);

            _currentRoutine = StartCoroutine(Pathing());
            _detectionModifier = stats.IdleStateDetectionModifier;
        }
        
        //This could be optimized via batching, if you're comfortable teaching that.
        private void Update()
        {
            _animator.SetFloat(StaticUtilities.MoveSpeedAnimID, _agent.velocity.magnitude);
            
            if (_isDetectionDecaying)
            {
                RemoveDetection(stats.DetectionDecayRate * Time.deltaTime);
            }
        }
        
        #region AILogic
        private IEnumerator Idle()
        {
            print("Entering Idle");
            _myState = EHumanState.Idle;
            yield return new WaitForSeconds(Random.Range(stats.MinIdleTime, stats.MaxIdleTime));
            _currentRoutine = StartCoroutine(Pathing());
        }

        private IEnumerator Pathing()
        {
            print("Entering Pathing");
            _pathHandler.SetNextPatrolPoint(); // Cringe....
            //This would work, but it checks every frame.
            _myState = EHumanState.Pathing;
            
            //We need to find a path, which is done in a different thread,
            //so the only way we can actually do this properly every time, is to wait until the thread merges main
            yield return new WaitWhile(() => _agent.pathPending);
            
            // While we have not reached our destination
            while(!_pathHandler.HasReachedDestination())
            {
                //Optimize by caching in start
                yield return TimerDelay;
            }
            
            _currentRoutine = StartCoroutine(Idle());
        }

        private IEnumerator Looking(Vector3 target)
        {
            print("Entering Looking");
            _myState = EHumanState.Looking;
            //When entering look state, we need to rotate to face the given location
            
            //and we need to start playing the look animation
            _animator.SetBool(StaticUtilities.IsSearchingAnimID, true);
            
            //Let's also modify our spotting multiplier 
            _detectionModifier = stats.LookingStateDetectionModifier;
            
            //Let's move towards the target, but not too close...
            _agent.SetDestination(Vector3.Lerp(transform.position, target, 0.1f));
            
            while(_currentDetection > 0f)
            {
                yield return TimerDelay;
            }
            
            //Reverse
            _animator.SetBool(StaticUtilities.IsSearchingAnimID, false);
            _detectionModifier = stats.IdleStateDetectionModifier;

            //Go back to track...
            if(_currentRoutine != null) StopCoroutine(_currentRoutine);
            _currentRoutine = StartCoroutine(Pathing());

        }
        
        //Can only be entered via detection.
        private void Chasing(Vector3 target)
        {
            print("Entering Chase");
            _myState = EHumanState.Chasing;
            
            //Disable the animation if it's still playing
            _animator.SetBool(StaticUtilities.IsSearchingAnimID, false);
            
            //When in chasing state, we will leave the path, and move towards the given location
            _agent.destination = target;
            
            //We also want to begin running, and therefore change the speed to run speed
            _agent.speed = stats.ChaseMoveSpeed;
            
            //While we are chasing, if the player gets to close to us, then they lose.
            
            //While chasing, we do not lose detection until we reach the last known location of the player...
            
            //We need to find a path, which is done in a different thread,
            //so the only way we can actually do this properly every time, is to wait until the thread merges main

        }
        #endregion

        #region Detection
        public float GetDetection()
        {
            return _currentDetection;
        }

        public void AddDetection(Vector3 location, float detection, EDetectionType detectionType)
        {
            //This line of code will allow us to ignore "stimuli" while chasing.
            if (_myState == EHumanState.Chasing && (detectionType & stats.IgnoreWhileChasing) != 0) return;
            
            _currentDetection = Mathf.Min(_currentDetection + detection * _detectionModifier, stats.MaxDetection);
            //print($"Adding detection, {location} --> {detection} * {_detectionModifier} + {_currentDetection} --> {_currentDetection}");
            float detectPerc = _currentDetection / stats.MaxDetection;
            _detectionBarMat.SetFloat(StaticUtilities.FillMatID, detectPerc);
            
            if (detectPerc >= 0.5f && _myState != EHumanState.Chasing && _myState != EHumanState.Looking)
            {
                //Enter looking state.
                if(_currentRoutine != null) StopCoroutine(_currentRoutine);
                _currentRoutine = StartCoroutine(Looking(location));
            }
            else if (detectPerc >= 1f)
            {
                //Begin Chase.
                if (_myState == EHumanState.Looking)
                {
                    //At this point cR cannot be null.
                    if(_currentRoutine != null) StopCoroutine(_currentRoutine);
                    Chasing(location);
                }
                
                //Update target location
                _agent.SetDestination(location);
            }
            
            //Restarting coroutines is expensive, it uses around 200 bytes of ram.
            //There are better ways to reset the timer, can you think of any?
            if(_decayTimer != null) StopCoroutine(_decayTimer);
            _decayTimer = StartCoroutine(BeginDecayCooldown());
        }
        
        private void RemoveDetection(float amount)
        {
            _currentDetection = Mathf.Max(_currentDetection - amount, 0);
            //print($"Removing detection, {_currentDetection} = {_currentDetection} - {amount}");

            float detectPerc = _currentDetection / stats.MaxDetection;
            _detectionBarMat.SetFloat(StaticUtilities.FillMatID, detectPerc);

            
            if (detectPerc <= 0.9f && _myState == EHumanState.Chasing)
            {
                StopCoroutine(_currentRoutine);
                _currentRoutine = StartCoroutine(Looking(_agent.destination));
                _agent.speed = stats.BaseMoveSpeed;
            }
            

            if (detectPerc == 0) _isDetectionDecaying = false;
        }
        
        private IEnumerator BeginDecayCooldown()
        {
            _isDetectionDecaying = false;
            yield return new WaitForSeconds(stats.BeginDecayCooldown);
            _isDetectionDecaying = true;
        }
        #endregion
    }
}
