using System.Collections;
using AI;
using Interfaces;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Characters
{
    public class AIChicken : Chicken, IDetector
    {
        [SerializeField] private FaceTarget faceTarget;
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
            PlayerChicken.onPlayerCaught += MoveToPlayer;
            PlayerChicken.onPlayerEscaped += MoveToPlayer;
            ++NumActiveAIChickens;
        }

        private void OnDisable()
        {
            print("Disabled: " + NumActiveAIChickens);
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
            faceTarget.enabled = true;
            StopAllCoroutines();
            PlayerChicken.onPlayerCaught -= MoveToPlayer;
            PlayerChicken.onPlayerEscaped -= MoveToPlayer;
          
            --NumActiveAIChickens;
        }

        //Without the player, let's just rush towards the player... If we get caught, oh well.
        private void MoveToPlayer(Vector3 obj)
        {
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
            WaitUntil target = new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance);

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
            HudManager.Instance.OnChickenCaptured();
            GameManager.PlayUISound(stats.OnCapture);

        }

        public void AddDetection(Vector3 location, float detection, EDetectionType type)
        {
            if (!enabled) return;
            _agent.SetDestination(location);
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
        }
    }
}
