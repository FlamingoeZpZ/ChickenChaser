using System;
using Interfaces;
using UnityEngine;

namespace Game
{
    public class LightZone : MonoBehaviour
    {
        [SerializeField] private float lightValue;
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody && other.TryGetComponent(out IVisualDetectable visible))
            {
                visible.AddVisibility(lightValue);   
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody && other.TryGetComponent(out IVisualDetectable visible))
            {
                visible.RemoveVisibility(lightValue);   
            }   
        }
    }
}
