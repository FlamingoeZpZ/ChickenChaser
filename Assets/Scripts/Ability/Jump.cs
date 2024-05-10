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

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected override float AbilityNum()
        {
            return 2;
        }

        protected override bool CanActivate()
        {
            return Owner.IsGrounded && base.CanActivate();
        }

        protected override void Activate()
        {
            //Apply upwards velocity to ourselves.
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        

    }
}
