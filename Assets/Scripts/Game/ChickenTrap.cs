using Characters;
using UnityEngine;
using Utilities;

namespace Game
{
    public class ChickenTrap : MonoBehaviour
    {
        [SerializeField] private float decayTime = .8f;
        private float _currentDecayTime;
        private Chicken _chicken;
        private Material _myMaterial;

        private void Awake()
        {
            //If we have the chicken, let's bind it... If we don't we must have been spawned in elsewhere.
            if(transform.GetChild(0).GetChild(0).TryGetComponent(out Chicken c)) AttachChicken(c);
            _myMaterial = GetComponent<MeshRenderer>().material;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.attachedRigidbody.TryGetComponent(out Chicken c) || !c.isActiveAndEnabled) return;
            _currentDecayTime += Time.deltaTime;
            _myMaterial.SetFloat(StaticUtilities.FillMatID, _currentDecayTime / decayTime);
            if (_currentDecayTime >= decayTime)
            {
                FreeChicken();
            }
        }

        private void FreeChicken()
        {
            _chicken.transform.parent = null;
            _chicken.ReleaseChicken();
            Destroy(gameObject);
        }

        public void AttachChicken(Chicken c)
        {
            _chicken = c;
            c.transform.parent = transform.GetChild(0);
            c.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            c.enabled = false; //Disabling the AI component, SHOULD automatically enable the secondary look at component
        }
    }
}
