using System;
using Game;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Characters
{
    public class HumanAnimatorReceiver : MonoBehaviour
    {
        [SerializeField] private CaptureZone zone;

        private AudioSource _source;
       
        
        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        //Used for animation sounds
        private void Step()
        {
            //If we missed, we can't possibly find a clip...
            Vector3 pos = transform.position + Vector3.up;
            
            
            //Make sure hit is not null
            if (!Physics.SphereCast(pos, 0.4f, Vector3.down, out RaycastHit hit, 1.4f,StaticUtilities.GroundLayers)) return;
            
            //Make sure the layer is not null
            if (!GameManager.SoundsDictionary.TryGetValue(hit.transform.tag, out AudioVolumeRangeSet set) || !_source) return;
            _source.pitch = Random.Range(0.8f, 1.2f);
            //Play the desired audio + detection
            _source.PlayOneShot(set.clip, set.volume);
            //Human steps SHOULD NOT be detectable by other humans
            //AudioDetection.onSoundPlayed.Invoke(pos, set.volume, set.rangeMultiplier, EAudioLayer.Human);
        }

        private void EnableCaptureZone()
        {
            print("Capture Zone Enabled");
            zone.enabled = true;
        }

        private void DisableCaptureZone()
        {
            print("Capture Zone Disabled");
            zone.enabled = false;
        }


        private void ThrowCapturedObject()
        {
            print("Throwing Object");
            zone.ThrowCaptureObject();
            zone.enabled = false;

        }

        private void OnDrawGizmosSelected()
        {
            GizmosExtras.DrawWireSphereCast(transform.position + Vector3.up, Vector3.down, 0.4f,1.4f);
        }
    }
}
