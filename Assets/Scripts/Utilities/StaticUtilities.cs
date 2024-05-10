using UnityEngine;

namespace Utilities
{
    public static class StaticUtilities
    {
        public static readonly int WallLayer = 1 << LayerMask.NameToLayer("Default");
        public static readonly int HumanLayer = 1 << LayerMask.NameToLayer("Human");
        public static readonly int PlayerLayer = 1 << LayerMask.NameToLayer("Player");
        public static readonly int BushLayer = 1 << LayerMask.NameToLayer("HidingZone");
        public static readonly int EverythingButChicken = ~(PlayerLayer | HumanLayer);
    
        public static readonly int VisibilityLayer =  WallLayer | HumanLayer ;
    
    
        //Animations
        public static readonly int AbilityTypeAnimID = Animator.StringToHash("abilityType");
        public static readonly int MoveSpeedAnimID = Animator.StringToHash("moveSpeed");
        public static readonly int AbilityAnimID = Animator.StringToHash("Ability");
        public static readonly int IsGroundedAnimID = Animator.StringToHash("isGrounded");
        public static readonly int IsSearchingAnimID = Animator.StringToHash("isSearching");
        public static readonly int CaptureAnimID = Animator.StringToHash("capture");
        public static readonly int BeginCaptureAnimID = Animator.StringToHash("beginCapture");


        public static readonly int FillMatID = Shader.PropertyToID("_Fill");
    }
}
