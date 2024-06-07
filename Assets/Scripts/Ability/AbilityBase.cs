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
        
        
        private bool TriggerAnimator() => AbilityTriggerID()!=0;
        private bool BoolAnimator() => AbilityBoolID()!=0;
        
        protected virtual int AbilityBoolID() => 0;
        protected virtual int AbilityTriggerID() => 0;

        
        protected Chicken Owner;
        protected Animator Animator;
        
        private bool _isReady = true;
        private bool _isBeingHeld;
        private float _currentCooldownTime;
        public Sprite Icon => icon;

        
        private IEnumerator BeginCooldown()
        {
            do
            {
                yield return new WaitUntil(CanActivate);
                if(!_isBeingHeld) yield break; // We should actually just quit if the user let go.
                Activate();
                if(TriggerAnimator()) Animator.SetTrigger(AbilityTriggerID());
                _currentCooldownTime = 0;
                _isReady = false;
                while (_currentCooldownTime < cooldown)
                {
                    _currentCooldownTime += Time.deltaTime;
                    yield return null;
                }

                //Make sure that it's running!
                _isReady = true;
                
            } while (_isBeingHeld && canBeHeld);

            StopAbility();
        }

        //Start because we want to use Awake in the others. Can also do virtual override optional
        private void Start()
        {
            Owner = GetComponentInParent<Chicken>();
            Animator = GetComponentInChildren<Animator>();
         
        }

        public void StartAbility()
        {
            _isBeingHeld = true;
            StartCoroutine(BeginCooldown());
            if(BoolAnimator()) Animator.SetBool(AbilityBoolID(), true);
        }

        public void StopAbility()
        {
            _isBeingHeld = false;
            if(BoolAnimator()) Animator.SetBool(AbilityBoolID() ,false);
        }




        public virtual bool CanActivate()
        {
            return _isReady;
        }

        protected abstract void Activate();

        public float GetReadyPercent()
        {
            return _currentCooldownTime / cooldown;
        }

        public virtual void ForceCancelAbility()
        {
            _currentCooldownTime = cooldown;
            _isReady = true;
            StopAllCoroutines();
            StopAbility();
        }
    }
}
