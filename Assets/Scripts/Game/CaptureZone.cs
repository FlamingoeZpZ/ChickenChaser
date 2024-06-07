using Characters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utilities;

namespace Game
{
    public class CaptureZone : MonoBehaviour
    {
        [SerializeField] private ThrowZone throwObject;
        [SerializeField] private Transform chickenPoint;
        [SerializeField] private float throwForce;

        private bool _isPendingCapture;
        private Collider _collider;
        private Human _human;
        private Animator _animator;
        private Chicken _caught;
        private void Awake()
        {
            _human = GetComponentInParent<Human>();
            _animator = GetComponentInParent<Animator>();
            _collider = GetComponent<Collider>();
        }

        //If when we're enabled, something has entered our trigger, then we know we've caught them
        private void OnTriggerEnter(Collider other)
        {
            //Firstly, let's check to see it's a chicken and that chicken is an active chicken
            if (other.attachedRigidbody.TryGetComponent(out _caught) && _caught.isActiveAndEnabled)
            {
                //From here, we need to disable the chicken
                _caught.enabled = false;
                
                //And attach it to our grapple point
                _caught.transform.SetParent(chickenPoint, true);
                _caught.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    
                //We then need to play the throw chicken animation
                _animator.SetTrigger(StaticUtilities.BeginCaptureAnimID);
                enabled = false;
                //We can spawn in a cage on the cage point and make the chicken a child of the cage
                

            }
        }


        private void OnEnable()
        {
            _collider.enabled = true;
        }

        //If we've been disabled, make s
        private void OnDisable()
        {
            _human.EndRoll();
            _collider.enabled = false;
        }

        public void ThrowCaptureObject()
        {
            //Imagine throwing a pokeball, we need to give full responsibility to the thrower object.
            //You could change this code so that you use an interface, ICapturable and in theory catch anything.
            
            var trap = Instantiate(throwObject, chickenPoint.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(trap.gameObject, SceneManager.GetSceneByBuildIndex(2));
            trap .Initialize(transform.forward * throwForce, _caught);

        }
    }
}
