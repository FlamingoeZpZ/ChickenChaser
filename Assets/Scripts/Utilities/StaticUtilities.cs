using System.Collections;
using System.Collections.Generic;
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
        public static readonly int ChickenAiLayer = 1 << LayerMask.NameToLayer("AiChicken");
        
        //Describes layers for detection
        public static readonly int EverythingButChicken = ~(PlayerLayer | HumanLayer | ChickenAiLayer);
        
        //Describes the layers we cannot see/pass through
        public static readonly int VisibilityLayer =  WallLayer | HumanLayer;
        
        //What layers are we looking for
        public static readonly int DetectableLayer = PlayerLayer | ChickenAiLayer;
        
        //Describes the layers that will count as grounded if we are in or touching
        public static readonly int GroundLayers = WallLayer | WaterLayer | BushLayer;
    
    
        //Animations
        public static readonly int MoveSpeedAnimID = Animator.StringToHash("moveSpeed");
        public static readonly int CluckAnimID = Animator.StringToHash("IsDancing");
        public static readonly int JumpAnimID = Animator.StringToHash("Jump");
        public static readonly int DashAnimID = Animator.StringToHash("Dash");
        
        public static readonly int IsGroundedAnimID = Animator.StringToHash("isGrounded");
        public static readonly int IsSearchingAnimID = Animator.StringToHash("isSearching");
        public static readonly int CaptureAnimID = Animator.StringToHash("Dive");
        public static readonly int BeginCaptureAnimID = Animator.StringToHash("HasChicken");


        public static readonly int FillMatID = Shader.PropertyToID("_Fill");

        private static float volume;

        private static float t = 0;
        //The "this" keyword will allow us to say source.TransitionSound anywhere.
        /// <summary>
        /// This is a coroutine function that transitions audio. It is not async because web support
        /// </summary>
        /// <param name="source"></param>
        /// <param name="nextClip"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static IEnumerator TransitionSound(this AudioSource source, AudioClip nextClip, float duration)
        {
            //If we're not active
            if (t == 0)
            {
                //use the volume of the source... But if we were already active, use the cached value...
                //ISSUE: both t and volume exist only once and therefore Transition sound onkly can work for one thing at a time.
                volume = source.volume;
            }
            
            bool hasChanged = false;
            while (t < duration)
            {
                t += Time.deltaTime;
                float percent = t / duration;
                
                //Make a parabolic function, in which when percent is 0, currentVolume is volume, and when percent is 0.5, volume is 0, and when percent is currentVolume is volume
                //x = 0, y = volume
                //x = 0.5, y = 0
                //x = 1, y = volume
                var currentVolume = 4 * volume * (percent - 0.5f) * (percent - 0.5f);

                if (!hasChanged && percent > 0.5f)
                {
                    hasChanged = true;
                    source.clip = nextClip;
                    source.Play();
                }
                source.volume = currentVolume;

                yield return null;
            }

            t = 0;
            source.volume = volume;
        }
    }
}
