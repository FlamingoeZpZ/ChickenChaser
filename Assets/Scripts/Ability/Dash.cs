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
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            SphereCollider c = GetComponentInChildren<SphereCollider>();
            _radius = c.radius * c.transform.lossyScale.x;
        }
    
        private IEnumerator ActivateAbility(Vector3 direction)
        {
            //Do not dash upwards.
            direction = new Vector3(direction.x, 0, direction.z).normalized;
            
            feathers.transform.forward = -direction; // Face the opposite direction of ourselves.
            feathers.Play();
            _canDash = false;
            Vector3 endPoint =
                Physics.SphereCast(transform.position, _radius, direction, out RaycastHit hit, dashDistance, StaticUtilities.VisibilityLayer)
                    ?  hit.point + hit.normal * (_radius*2)
                    : direction * dashDistance + transform.position;
            Debug.DrawLine(transform.position, endPoint, Color.magenta, 5);
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
