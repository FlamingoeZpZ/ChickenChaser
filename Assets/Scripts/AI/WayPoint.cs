using UnityEngine;

namespace AI
{
    /// <summary>
    /// To optimize this script, create a method to auto generate a scriptable object and cache the values.
    /// </summary>
    public class WayPoint : MonoBehaviour
    {
        [SerializeField] private float suggestedDelay;
        private Vector3 _forward;
        private Vector3 _position;
        
        // (Make a private variable accessible, but not editable in other files)
        public float SuggestedDelay => suggestedDelay;
        public Vector3 Forward => _forward;
        public Vector3 Position => _position;
        
        private static readonly Color Orange = new Color(1F, 0.5F, 0);


        private void Awake()
        {
            _position = transform.position;
            _forward = transform.forward;
         
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Orange;
            if(suggestedDelay > 0)
                Gizmos.DrawRay(transform.position, transform.forward * Mathf.Max(1,suggestedDelay));
        }
    }
}
