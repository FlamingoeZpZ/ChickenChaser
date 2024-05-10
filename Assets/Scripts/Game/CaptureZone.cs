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
        
        public static Action onGameLoss;

        private void Awake()
        {
            TryGetComponent(out _animator);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_original && other != _original) return;
            if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Chicken _))
            {
                if (!_isPendingCapture) StartCoroutine(Delay());
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

        private IEnumerator Delay()
        {
            if (_animator)
            {
                _animator.SetTrigger(StaticUtilities.BeginCaptureAnimID);
            }
            yield return new WaitForSeconds(delay);
            if (_isPendingCapture)
            {
                onGameLoss.Invoke();
                if (_animator)
                {
                    _animator.SetTrigger(StaticUtilities.CaptureAnimID);
                }
            }
        }

    }
}
