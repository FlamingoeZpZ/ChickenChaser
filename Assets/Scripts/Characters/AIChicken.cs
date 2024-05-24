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
          
        }

        private void OnDisable()
        {
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
            faceTarget.enabled = true;
            StopAllCoroutines();
     
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
            
            
            HudManager.Instance.OnChickenEscaped();
        }

        public override void CaptureChicken()
        {
            enabled = false;
            HudManager.Instance.OnChickenCaptured();
        }

        public void AddDetection(Vector3 location, float detection, EDetectionType type)
        {
            if (!enabled) return;
            _agent.SetDestination(location);
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
        }
    }
}
