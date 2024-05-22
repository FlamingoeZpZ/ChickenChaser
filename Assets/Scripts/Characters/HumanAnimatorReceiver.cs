using System;
using AI;
using Managers;
using ScriptableObjects;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Characters
{
    public class HumanAnimatorReceiver : MonoBehaviour
    {
        private AudioSource _source;
        private void Awake()
        {
            _source = GetComponentInParent<AudioSource>();
        }

        //Used for animation sounds
        private void Step()
        {
            print("Human stepped");
            //If we missed, we can't possibly find a clip...
            Vector3 pos = transform.position;
            
            //Make sure hit is not null
            if (!Physics.SphereCast(pos, 0.4f, Vector3.down, out RaycastHit hit, 0.4f,StaticUtilities.GroundLayers)) return;
            
            //Make sure the layer is not null
            if (!GameManager.SoundsDictionary.TryGetValue(hit.transform.tag, out AudioVolumeRangeSet set)) return;
            _source.pitch = Random.Range(0.8f, 1.2f);
            //Play the desired audio + detection
            _source.PlayOneShot(set.clip, set.volume);
            AudioDetection.onSoundPlayed.Invoke(pos, set.volume, set.rangeMultiplier, EAudioLayer.Human);
        }

    }
}
