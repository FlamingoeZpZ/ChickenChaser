using System.Collections;
using UnityEngine;
using Utilities;

namespace Ability
{
    public class Dash : AbilityBase
    {
        [Header("Dash")] 
        [SerializeField] private float dashDistance;
        [SerializeField] private float dashDuration;

        [Header("Effects")]
        [SerializeField] private ParticleSystem feathers;
        
        private Rigidbody _rb;
        private bool _canDash = true;
        private float _radius;

        private const int numSamples = 10;
        private static readonly Vector3 maxVerical = new Vector3(0,1,0);
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            SphereCollider c = GetComponentInChildren<SphereCollider>();
            _radius = c.radius * c.transform.lossyScale.x;
        }
    
        private IEnumerator ActivateAbility(Vector3 direction)
        {
            //Do not dash upwards.
            direction = new Vector3(direction.x, 0, direction.z) * dashDistance;
            _canDash = false;
            
            //Find the highest point on a slope
            Vector3 origin = transform.position;
            float yMax = 0;
            Vector3 start = origin + maxVerical;
            for (int i = 0; i < numSamples; ++i)
            {
                float percent = (float)i / numSamples;
                if (Physics.Raycast(start + direction * percent, Vector3.down,
                        out RaycastHit targetHit,
                        dashDistance, StaticUtilities.VisibilityLayer))
                {
                    Debug.DrawRay(start + direction * percent,Vector3.down *  dashDistance,(targetHit.point.y > yMax)?Color.green:Color.red, 3 );
                    if (targetHit.point.y > yMax) yMax = targetHit.point.y;
                }
                else
                {
                    Debug.DrawRay(start + direction * percent,Vector3.down *  dashDistance,Color.red, 3 );
                }
            }

            // Make sure going upwards actually makes sense by subtracting the goal Y from the current y. Then add back the radius, so we have the true end point.
            float d = yMax - origin.y + _radius *2;
            //If it was negative, or 0 just go forward. else, set the target D
            if(d > 0) direction.y = d; 
            
            Debug.Log($"Dashing in direction: {direction}, {yMax} - {origin.y}");
            direction = direction.normalized;
            
            feathers.transform.forward = -direction; // Face the opposite direction of ourselves.
            feathers.Play();
            
            Vector3 endPoint =
                Physics.SphereCast(origin, _radius, direction, out RaycastHit hit, dashDistance, StaticUtilities.VisibilityLayer)
                    ?  hit.point + hit.normal * (_radius*2)
                    : direction * dashDistance + transform.position;
            Debug.DrawLine(origin, endPoint, Color.magenta, 5);
            float curTime = 0;
            _rb.isKinematic = true;
            while (curTime < dashDuration) 
            {
                curTime += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, endPoint, curTime/dashDuration);
                yield return null;
            }
            transform.position = Vector3.Lerp(transform.position, endPoint, 1);
            _rb.isKinematic = false;
            feathers.Stop();
            _canDash = true;
            
        }

        protected override float AbilityNum()
        {
            return 0;
        }

        protected override bool CanActivate()
        {
            //Partially redundant, but you can have an additional cooldown this way.
            return _canDash && base.CanActivate();
        }

        protected override void Activate()
        {
            StartCoroutine(ActivateAbility(Owner.HeadForward));
        }


        private void OnDrawGizmosSelected()
        {
            SphereCollider c = GetComponentInChildren<SphereCollider>();
            _radius = c.radius * c.transform.lossyScale.x;
            
            Gizmos.color = Color.yellow;
            GizmosExtras.DrawWireSphereCast(transform.position, transform.forward, dashDistance, _radius);
        }
    }
}
