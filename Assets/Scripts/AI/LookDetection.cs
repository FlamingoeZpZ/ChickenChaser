using Interfaces;
using ScriptableObjects;
using UnityEngine;
using Utilities;

//We make this script, not because we're worried about optimization, but because it helps keep the code clean,
//and allows us to expand the project, if we wanted to add new types of AI
namespace AI
{
    public class LookDetector : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private LookStats stats;
        private IDetector _detector;
        private readonly Collider[] _maxDetections = new Collider[3];
        private void Awake()
        {
            _detector = GetComponent<IDetector>();
        }

        private void Update()
        {
            LookDetection();
        }

        void LookDetection()
        {
            //We need to KNOW where every target is...
            //Take the dot product from our direction to the player position...
            Vector3 fwd = head.forward;
            Vector3 pos = head.position;
            
            int n = Physics.OverlapSphereNonAlloc(pos, stats.VisionDistance, _maxDetections, StaticUtilities.DetectableLayer);
            if (n == 0) return;

            for (int i = 0; i < n; ++i)
            {
                if (!_maxDetections[i].attachedRigidbody || !_maxDetections[i].attachedRigidbody.TryGetComponent(out IVisualDetectable detectable)) continue;
                Vector3 targetPosition = _maxDetections[i].transform.position;
                Vector3 playerLookDirection = targetPosition - pos;
                float distance = playerLookDirection.magnitude;
                Vector3 normal = playerLookDirection / distance;
                float dot = Vector3.Dot(fwd, normal);

                if (dot > stats.VisionFOV && distance < stats.VisionDistance)
                {
                    #if UNITY_EDITOR
                    Debug.DrawRay(pos, normal * distance);
                    #endif
                    //If we fail to detect any blocking layers.
                    if (!Physics.Raycast(pos, normal, out RaycastHit _, distance, StaticUtilities.EverythingButChicken))
                    {
                        float distancePerc = 1 - (distance / stats.VisionDistance);
                        _detector.AddDetection(targetPosition, stats.SightDetectionValue(distancePerc) * detectable.GetVisibility(), EDetectionType.Visual);

                    }
                }
            }
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(head.position, head.forward * stats.VisionDistance);
            //Dot product -- 1 in front, 0 sides, -1 behind.
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(stats.VisionFOV) * Mathf.Rad2Deg , head.right) * head.forward * stats.VisionDistance);
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(stats.VisionFOV) * Mathf.Rad2Deg , -head.right) * head.forward * stats.VisionDistance);
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(stats.VisionFOV) * Mathf.Rad2Deg , head.up) * head.forward * stats.VisionDistance);
            Gizmos.DrawRay(head.position, Quaternion.AngleAxis(Mathf.Acos(stats.VisionFOV) * Mathf.Rad2Deg , -head.up) * head.forward * stats.VisionDistance);

        }
    }
}
