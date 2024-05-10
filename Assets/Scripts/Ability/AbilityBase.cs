using System;
using System.Collections;
using Characters;
using UnityEngine;
using UnityEngine.Events;

namespace Ability
{
    public abstract class AbilityBase : MonoBehaviour
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private float cooldown;
        
        protected Chicken Owner;
        
        private bool _isReady = true;
        
        private float _currentCooldownTime;
        private Action<float> _onAbilityUpdated;

        public Sprite Icon => icon;

        public void BindAbilityUpdated(Action<float> action)
        {
            _onAbilityUpdated += action;
        }



        private IEnumerator BeginCooldown()
        {
            _currentCooldownTime = 0;
            _isReady = false;
            while (_currentCooldownTime < cooldown)
            {
                _onAbilityUpdated?.Invoke(_currentCooldownTime / cooldown);
                _currentCooldownTime += Time.deltaTime;
                yield return null;
            }
            //Make sure that it's running!
            _onAbilityUpdated?.Invoke(1);
            _isReady = true;
        }
        
        public float BindOwner(Chicken newOwner)
        {
            Owner = newOwner;
            return AbilityNum();
        }
        
        public bool TryAbility()
        {
            //can I dash?
            if (!CanActivate()) return false;
            Activate();
            StartCoroutine(BeginCooldown());
            return true;
        }
        
        protected abstract float AbilityNum();

        protected virtual bool CanActivate()
        {
            return _isReady;
        }

        protected abstract void Activate();

    }
}
