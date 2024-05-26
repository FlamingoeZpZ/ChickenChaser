using System;
using System.Collections;
using Characters;
using UnityEngine;
using Utilities;

namespace Game
{
    public class CaptureZone : MonoBehaviour
    {
        [SerializeField] private float delay;
        private bool _isPendingCapture;
        private Collider _original;
        private Animator _animator;

        private void Awake()
        {
            TryGetComponent(out _animator);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_original && other != _original) return;
            if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Chicken c))
            {
                if (!_isPendingCapture) StartCoroutine(Delay(c));
                _isPendingCapture = true;
                _original = other;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == _original)
            {
                _original = null;
                _isPendingCapture = false;
            }
        }

        private IEnumerator Delay(Chicken c)
        {
            if (_animator)
            {
                _animator.SetTrigger(StaticUtilities.BeginCaptureAnimID);
            }
            yield return new WaitForSeconds(delay);
            if (_isPendingCapture)
            {
                c.CaptureChicken();
                if (_animator)
                {
                    _animator.SetTrigger(StaticUtilities.CaptureAnimID);
                }
            }
        }

    }
}
