using System;
using System.Collections;
using Characters;
using UnityEngine;

namespace Ability
{
    public class Jump : AbilityBase
    {
        [Header("Jump")] 
        [SerializeField] private float jumpForce;


        
        protected override float AbilityNum()
        {
            return 2;
        }

        protected override bool CanActivate()
        {
            return Owner.IsGrounded && IsReady;
        }

        protected override void Activate()
        {
            //Apply upwards velocity to ourselves.
            
        }

        

    }
}
