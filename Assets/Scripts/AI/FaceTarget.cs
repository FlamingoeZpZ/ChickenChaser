using Interfaces;
using UnityEngine;

namespace AI
{
    public class FaceTarget : MonoBehaviour, IDetector
    {
        public Vector3 suggestedForward;
        [SerializeField] private float rotationSpeed = 5;

        private void Awake()
        {
            suggestedForward = transform.forward;
        }

        private void LateUpdate()
        {
            LookAtTarget();
        }
        
        void LookAtTarget()
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(suggestedForward.x, 0, suggestedForward.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        public void AddDetection(Vector3 location, float detection, EDetectionType type)
        {
            //Look at our target
            suggestedForward = (location - transform.position).normalized;
        }
    }
}
