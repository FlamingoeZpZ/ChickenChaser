using System;
using System.Collections;
using Characters;
using UnityEngine;

namespace Ability
{
    public abstract class AbilityBase : MonoBehaviour
    {
        
        [SerializeField] private float cooldown;
        
        protected bool IsReady = true;
        protected Chicken Owner;
        
        private WaitForSeconds _cooldownDelay;


        private void Awake()
        {
            _cooldownDelay = new WaitForSeconds(cooldown);
        }

        private IEnumerator BeginCooldown()
        {
            IsReady = false;
            yield return _cooldownDelay;
            IsReady = true;
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

        protected abstract bool CanActivate();

        protected abstract void Activate();
        
        
        

    }
}
