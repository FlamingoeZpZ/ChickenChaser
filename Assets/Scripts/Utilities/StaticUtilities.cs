using UnityEngine;

namespace Utilities
{
    public static class StaticUtilities
    {
        public static readonly int WallLayer = 1 << LayerMask.NameToLayer("Default");
        
        public static readonly int WaterLayer = 1 << LayerMask.NameToLayer("Water");
        public static readonly int BushLayer = 1 << LayerMask.NameToLayer("HidingZone");
        
        
        public static readonly int HumanLayer = 1 << LayerMask.NameToLayer("Human");
        public static readonly int PlayerLayer = 1 << LayerMask.NameToLayer("Player");
        
        //Describes layers for detection
        public static readonly int EverythingButChicken = ~(PlayerLayer | HumanLayer);
        
        //Describes the layers we cannot see/pass through
        public static readonly int VisibilityLayer =  WallLayer | HumanLayer;
        
        //Describes the layers that will count as grounded if we are in or touching
        public static readonly int GroundLayers = WallLayer | WaterLayer | BushLayer;
    
    
        //Animations
        public static readonly int MoveSpeedAnimID = Animator.StringToHash("moveSpeed");
        public static readonly int CluckAnimID = Animator.StringToHash("IsDancing");
        public static readonly int JumpAnimID = Animator.StringToHash("Jump");
        public static readonly int DashAnimID = Animator.StringToHash("Dash");
        public static readonly int IsGroundedAnimID = Animator.StringToHash("isGrounded");
        public static readonly int IsSearchingAnimID = Animator.StringToHash("isSearching");
        public static readonly int CaptureAnimID = Animator.StringToHash("capture");
        public static readonly int BeginCaptureAnimID = Animator.StringToHash("beginCapture");


        public static readonly int FillMatID = Shader.PropertyToID("_Fill");
    }
}
