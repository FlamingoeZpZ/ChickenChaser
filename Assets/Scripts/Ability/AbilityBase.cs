using System;
using System.Collections;
using Characters;
using UnityEngine;
namespace Ability
{
    public abstract class AbilityBase : MonoBehaviour
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private float cooldown;
        [SerializeField] private bool canBeHeld;
        
        protected Chicken Owner;
        private bool _isReady = true;
        private bool _isBeingHeld;
        private float _currentCooldownTime;
        private Action<float> _onAbilityUpdated;
        

        public Sprite Icon => icon;

        public void BindAbilityUpdated(Action<float> action)
        {
            _onAbilityUpdated += action;
        }
        
        private IEnumerator BeginCooldown()
        {
            do
            {
                yield return new WaitUntil(CanActivate);
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
                Activate();
            } while (_isBeingHeld && canBeHeld);
        }

        //Start because we want to use Awake in the others. Can also do virtual override optional
        private void Start()
        {
            Owner = GetComponentInParent<Chicken>();
        }

        public void StartAbility()
        {
            _isBeingHeld = true;
            StartCoroutine(BeginCooldown());
            

        }

        public void StopAbility()
        {
            _isBeingHeld = false;
        }


        public abstract int AbilityNum();

        protected virtual bool CanActivate()
        {
            return _isReady;
        }

        protected abstract void Activate();

    }
}
