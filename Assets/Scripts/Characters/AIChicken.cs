using AI;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Characters
{
    public class AIChicken : Chicken, IDetector
    {
        [SerializeField] private FaceTarget faceTarget;
        private NavMeshAgent _agent;
        
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            
            _agent.speed = stats.MaxSpeed;
            _agent.acceleration = stats.Speed;
           
        }

        //public static
        private void OnEnable()
        {
            Animator.SetBool(StaticUtilities.CluckAnimID, true);
            faceTarget.enabled = false;
        }

        private void OnDisable()
        {
            //Initialize Looker Component so we look at the player
            //TryCluck();
            Animator.SetBool(StaticUtilities.CluckAnimID, false);
            faceTarget.enabled = true;
        }

        public void AddDetection(Vector3 location, float detection, EDetectionType type)
        {
            if (!enabled) return;
            _agent.SetDestination(location);
        }
    }
}
