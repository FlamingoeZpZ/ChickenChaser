using System;
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
        private bool _isOpened;
        private void Awake()
        {
            if (transform.childCount > 0 && transform.GetChild(0).childCount > 0 &&
                transform.GetChild(0).GetChild(0).TryGetComponent(out _chicken))
            {
                _chicken.transform.parent = transform.GetChild(0);
                _chicken.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _chicken.enabled = false; //Disabling the AI component, SHOULD automatically enable the secondary look at component
            }

            _myMaterial = GetComponent<MeshRenderer>().material;
        }

        private void OnTriggerStay(Collider other)
        {
            //When the chicken is freed, its triggering this again, and freeing itself twice because OnTriggerStay runs on the physics ticks
            if (!_chicken  || !other.attachedRigidbody.TryGetComponent(out Chicken c) || !c.isActiveAndEnabled || _isOpened) return;
            _currentDecayTime += Time.deltaTime * 2;
            _myMaterial.SetFloat(StaticUtilities.FillMatID, _currentDecayTime / decayTime);
            if (_currentDecayTime >= decayTime)
            {
                _isOpened = true;
                FreeChicken();
            }
        }

        private void LateUpdate()
        {
            if(_isOpened || _currentDecayTime <= 0) return;
            _currentDecayTime -= Time.deltaTime;
            if (_currentDecayTime <= 0) _currentDecayTime = 0;
            _myMaterial.SetFloat(StaticUtilities.FillMatID, _currentDecayTime / decayTime);
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
            c.CaptureChicken();
        }
    }
}
