using System.Collections;
using AI;
using Interfaces;
using Managers;
using ScriptableObjects;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Characters
{
    public class AIChicken : Chicken, IDetector
    {
        [SerializeField] private FaceTarget faceTarget;
        [SerializeField] private AudioDetection detection;
        [SerializeField] private HearStats trappedHearing;
        [SerializeField] private HearStats regularHearing;
        private NavMeshAgent _agent;
        

        public static int NumActiveAIChickens { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponent<NavMeshAgent>();
        
            _agent.speed = stats.MaxSpeed;
            _agent.acceleration = stats.Speed;

            //Each Chicken needs to be registered.
            GameManager.RegisterAIChicken();
            
            //Draw them in the UI too.
            HudManager.Instance.RegisterChicken();
        }


        //public static
        private void OnEnable()
        {
            Animator.SetBool(StaticUtilities.CluckAnimID, true);
            faceTarget.enabled = false;
            //Rb.isKinematic = false;
            PlayerChicken.onPlayerCaught += MoveToPlayer;
            PlayerChicken.onPlayerEscaped += MoveToPlayer;
            ++NumActiveAIChickens;
            _agent.enabled = true;
            _collider.enabled = true;
            detection.SetStats(regularHearing);
        }

        private void OnDisable()
        {
            print("Disabled: " + NumActiveAIChickens);
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
            faceTarget.enabled = true;
            StopAllCoroutines();
            PlayerChicken.onPlayerCaught -= MoveToPlayer;
            PlayerChicken.onPlayerEscaped -= MoveToPlayer;
            //Rb.isKinematic = true;
            --NumActiveAIChickens;
            //Cancel all previous instructions
            _agent.ResetPath();
            _agent.enabled = false;
            _collider.enabled = false;
            detection.SetStats(trappedHearing);

            
            
            
            Move(Vector2.zero);
            Look(Vector2.zero);
        }

        //Without the player, let's just rush towards the player... If we get caught, oh well.
        private void MoveToPlayer(Vector3 obj)
        {
            print("I'm moving to the player");
            _agent.SetDestination(obj);
        }

        protected override void HandleMovement()
        {
            moveSpeed = Mathf.Max(0,_agent.remainingDistance - _agent.stoppingDistance + 0.2f); //Cheesy workaround !
            Animator.SetFloat(StaticUtilities.MoveSpeedAnimID, moveSpeed);
        }

        public override void ReleaseChicken()
        {
            enabled = true;
            Animator.enabled = true;
            HudManager.Instance.OnChickenRescued();
        }

        public override void EscapeAndMoveTo(Vector3 position)
        {
            print("I wanna get out!");
            //Move to the location to escape
            _agent.SetDestination(position);
            
            //We should not escape just yet because the AI needs time to actually get to the exit...
            //let's start a coroutine and see if we've escaped.
            StartCoroutine(CheckForEscaped());
            
            
        }

        private IEnumerator CheckForEscaped()
        {
            //CACHED Move until we reach the target
            WaitUntil target = new WaitUntil(() => _agent.hasPath && _agent.remainingDistance <= _agent.stoppingDistance);

            //Use cached variable
            yield return target;
            
            print("I Escaped!");
            
            //Then we've successfully escaped, so let's update the GameManager, who will update the UI.
            GameManager.RegisterAIEscape();
            GameManager.PlayUISound(stats.OnEscape);
            
            //Giga Cheesy, but we need to make sure that the num chickos is right.
            --NumActiveAIChickens;
            HudManager.Instance.OnChickenEscaped();
            ++NumActiveAIChickens;
            Destroy(gameObject);
            
        }

        public override void CaptureChicken()
        {
            enabled = false;
            Animator.SetFloat(StaticUtilities.MoveSpeedAnimID, 0);
            Animator.enabled = false;
            HudManager.Instance.OnChickenCaptured();
            GameManager.PlayUISound(stats.OnCapture);
    

        }

        public void AddDetection(Vector3 location, float detection, EDetectionType type)
        {
            if (!enabled || detection < 1) return;
            print("I'm moving towards: " + location);
            _agent.SetDestination(location);
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
        }
    }
}
