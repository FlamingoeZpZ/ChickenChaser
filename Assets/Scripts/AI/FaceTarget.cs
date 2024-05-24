using Interfaces;
using UnityEngine;

namespace AI
{
    public class FaceTarget : MonoBehaviour, IDetector
    {
        private Vector3 _suggestedForward;
        [SerializeField] private float rotationSpeed = 5;

        private void Awake()
        {
            _suggestedForward = transform.forward;
        }

        private void LateUpdate()
        {
            LookAtTarget();
        }
        
        void LookAtTarget()
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(_suggestedForward.x, 0, _suggestedForward.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        public void AddDetection(Vector3 location, float detection, EDetectionType type)
        {
            //Look at our target
            _suggestedForward = (location - transform.position).normalized;
        }
    }
}
