using UnityEngine;
using Utilities;

namespace Game
{
    public class ChickenTrap : MonoBehaviour
    {
        [SerializeField] private float decayTime = .8f;
        private float _currentDecayTime;
        private ITrappable _trappable;
        private Material _myMaterial;
        private bool _isOpened;
        private void Awake()
        {
            if (transform.childCount > 0 && transform.GetChild(0).childCount > 0 &&
                transform.GetChild(0).GetChild(0).TryGetComponent(out _trappable))
            {
                PauseObject();
            }

            _myMaterial = GetComponent<MeshRenderer>().material;
        }

        private void OnTriggerStay(Collider other)
        {
            //When the chicken is freed, its triggering this again, and freeing itself twice because OnTriggerStay runs on the physics ticks
            if (_trappable == null  || !other.attachedRigidbody.TryGetComponent(out ITrappable c) || !c.CanBeTrapped() || _isOpened) return;
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
            _trappable.GetTransform().parent = null;
            _trappable.OnReleased();
            Destroy(gameObject);
        }

        public void AttachChicken(ITrappable c)
        {
            _trappable = c;
            PauseObject();
            _trappable.OnCaptured();
        }

        private void PauseObject()
        {
            Transform tr = _trappable.GetTransform();
            tr.parent = transform.GetChild(0);
            tr.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _trappable.OnPreCapture(); //Disabling the AI component, SHOULD automatically enable the secondary look at component
        }
    }
}
