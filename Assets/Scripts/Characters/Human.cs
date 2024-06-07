using System;
using System.Collections;
using AI;
using Interfaces;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Human : MonoBehaviour, IDetector
    {
        public enum EHumanState
        {
            Idle, // From idle, after X seconds, we begin pathing. If we see the chicken, enter looking state.
            Pathing, // From pathing, we follow the track until we reach our destination. If we reach the destination, enter idle for X seconds. If we see the chicken, enter looking state
            Looking, // Do not move, if line of sight remains for Y seconds, begin chasing, else enter pathing
            Chasing, // Chase the player, if line of sight is lost, enter the looking state.
            Rolling
        }
        
        [SerializeField] private AiStats stats;

        [Header("In Game")] 
        [SerializeField] private MeshRenderer detectionBar;
        private Material _detectionBarMat;

        private float _currentDetection;
        private EHumanState _myState;
        private bool _isDetectionDecaying;
        private float _detectionModifier;
        private float _previousSuggestedDelay;
        private Vector3 suggestedForward;
        private float _cachedStoppingDistance;

        private float timeSinceLastSpoken;

        private static readonly WaitForSeconds TimerDelay = new WaitForSeconds(0.016f);
        
        private Animator _animator;
        private PathHandler _pathHandler;
        private NavMeshAgent _agent;
        private AudioSource _source;

        private Coroutine _decayTimer;
        private Coroutine _currentRoutine;

        private void Awake()
        {
            _detectionBarMat = detectionBar.material;
            _animator = GetComponentInChildren<Animator>();
            _pathHandler = GetComponent<PathHandler>();
            _agent = GetComponent<NavMeshAgent>();
            _source = GetComponentInChildren<AudioSource>();

        }

        //Keep these in start as they are asking other scripts and objects to do things.
        //And we want to give them time to awaken as well.
        private void Start()
        {
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

            if (_myState is EHumanState.Idle or EHumanState.Looking or EHumanState.Rolling) FaceTarget();


        }
        
        void FaceTarget()
        {
            if (suggestedForward == Vector3.zero) return;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(suggestedForward.x, 0, suggestedForward.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * stats.BaseMoveSpeed);
        }

        
        #region AILogic
        
        private IEnumerator Idle()
        {
            _myState = EHumanState.Idle;
            if (_previousSuggestedDelay != 0)
            {
                suggestedForward = _pathHandler.GetSuggestedForward();
                yield return new WaitForSeconds(Random.Range(stats.MinIdleTime + _previousSuggestedDelay,
                    stats.MaxIdleTime + _previousSuggestedDelay));

            }

            _currentRoutine = StartCoroutine(Pathing());
        }

        private IEnumerator Pathing()
        {
            _pathHandler.SetNextPatrolPoint(); // Cringe....
            //This would work, but it checks every frame.
            _myState = EHumanState.Pathing;
            
            //We need to find a path, which is done in a different thread,
            //so the only way we can actually do this properly every time, is to wait until the thread merges main
            yield return new WaitWhile(() => _agent.pathPending);
            
            // While we have not reached our destination
            while(!_pathHandler.HasReachedDestination(out _previousSuggestedDelay))
            {
                //Optimize by caching in start
                yield return TimerDelay;
            }
            _currentRoutine = StartCoroutine(Idle());
        }

        private IEnumerator Looking(Vector3 target)
        {
            _myState = EHumanState.Looking;
            _agent.speed = stats.ChaseMoveSpeed;
            
            
            _agent.isStopped = true;
            
            float nt = Time.timeSinceLevelLoad;
            if (nt - stats.TimeNeededToTalk >= timeSinceLastSpoken)
            {
                timeSinceLastSpoken = nt;
                _source.PlayOneShot(stats.GetRandomHuh(), SettingsManager.currentSettings.SoundVolume * stats.HuhLoudness);
                //This cannot run, it will infinitely HUH because they detect their own noises as investigation sources
                //AudioManager.onSoundPlayed.Invoke(transform.position, stats.HuhLoudness, stats.HuhLoudness * 5);
            }
          
            
            
            //and we need to start playing the look animation
            _animator.SetBool(StaticUtilities.IsSearchingAnimID, true);
            
            //Let's also modify our spotting multiplier 
            _detectionModifier = stats.LookingStateDetectionModifier;

            suggestedForward = target - transform.position;
            
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
            _agent.speed = stats.BaseMoveSpeed;
            _agent.isStopped = false;

        }
        
        //Can only be entered via detection.
        private void Chasing(Vector3 target)
        {
            _agent.isStopped = false;
            timeSinceLastSpoken = Time.timeSinceLevelLoad;
            _myState = EHumanState.Chasing;
            
            _source.PlayOneShot(stats.GetRandomHey(), SettingsManager.currentSettings.SoundVolume * stats.HeyLoudness);
            AudioDetection.onSoundPlayed.Invoke(transform.position, stats.HeyLoudness, stats.HeyLoudness * 10, EAudioLayer.Human);
            
            
            //Disable the animation if it's still playing
            _animator.SetBool(StaticUtilities.IsSearchingAnimID, false);
            
            //When in chasing state, we will leave the path, and move towards the given location
            _agent.destination = target;
            
           

            AIManager.BeginChasing();

        }

        public void EndRoll()
        {
            _agent.speed = stats.ChaseMoveSpeed;
            _myState = EHumanState.Chasing;
            RemoveDetection(40);
            _agent.autoBraking = true;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            _agent.stoppingDistance = _cachedStoppingDistance;
            _agent.SetDestination(transform.position);
        }
        


        #endregion

        #region Detection

        public void AddDetection(Vector3 location, float detection, EDetectionType detectionType)
        {
            //This line of code will allow us to ignore "stimuli" while chasing.
            if ( _myState == EHumanState.Rolling || _myState == EHumanState.Chasing && (detectionType & stats.IgnoreWhileChasing) != 0) return;
            
            _currentDetection = Mathf.Min(_currentDetection + detection * _detectionModifier, stats.MaxDetection);
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
                
                //Let's calculate the distance from us and the target.

                Vector3 direction = (location - transform.position);
                
                float distance = direction.magnitude;

                if (distance <= stats.DiveDistance)
                {
                    //Play roll animation
                    _animator.SetTrigger(StaticUtilities.CaptureAnimID);
                
                    //Prevent future destination changes until the roll is complete.
                    _myState = EHumanState.Rolling;


                    suggestedForward = direction;
                    Debug.DrawLine(transform.position, transform.position + direction / distance * 8f, Color.red, 10, false);
                    
                     _agent.SetDestination(transform.position + direction / distance * (distance + 1));
                     
                     _agent.autoBraking = false;
                     _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                     _cachedStoppingDistance = _agent.stoppingDistance;
                     _agent.stoppingDistance = 0;
                     
                    _agent.speed = stats.ChaseMoveSpeed * 4f;

                }
            }
            
            //Restarting coroutines is expensive, it uses around 200 bytes of ram.
            //There are better ways to reset the timer, can you think of any?
            if(_decayTimer != null) StopCoroutine(_decayTimer);
            _decayTimer = StartCoroutine(BeginDecayCooldown());
        }
        
        private void RemoveDetection(float amount)
        {
            if (_myState == EHumanState.Rolling) return;
            
            _currentDetection = Mathf.Max(_currentDetection - amount, 0);

            float detectPerc = _currentDetection / stats.MaxDetection;
            _detectionBarMat.SetFloat(StaticUtilities.FillMatID, detectPerc);

            
            if (detectPerc <= 0.9f && _myState == EHumanState.Chasing)
            {
                StopCoroutine(_currentRoutine);
                _currentRoutine = StartCoroutine(Looking(_agent.destination));
                AIManager.StopChasing();
               
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
