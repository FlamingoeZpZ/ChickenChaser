using System;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

//We make this script, not because we're worried about optimization, but because it helps keep the code clean,
//and allows us to expand the project, if we wanted to add new types of AI
namespace AI
{
    public class AudioDetection : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private HearStats stats;
        private IDetector[] _detectors;
        
        public static Action<Vector3, float, float, EAudioLayer> onSoundPlayed;

        
        private void Awake()
        {
            _detectors = GetComponentsInChildren<IDetector>();
        }

        //We want our component to be toggleable, therefore, we need to be able to turn ourselves on and off.
        private void OnEnable()
        {
            //Subscribe when enabled
            onSoundPlayed += CheckListen;
        }

        private void OnDisable()
        {
            //Unsub when disabled
            onSoundPlayed -= CheckListen;
        }

        private void CheckListen(Vector3 location, float volume, float range, EAudioLayer layer)
        {
            if (!stats.CanHearSound(layer)) return;
            
            //Get the distance between us and the location
            float distance = Vector3.Distance(head.position, location);

            if (distance > range) return;
            
            //Sample the curve, based on the distance. (1 is close, and 0 is far.)

            float percentDistance =  1 - (distance / stats.AudioRange) * volume;

            foreach (var detector in _detectors)
                detector.AddDetection(location, stats.AudioDetectionValue(percentDistance), EDetectionType.Audio);
        
            Debug.DrawLine(head.position,location, Color.yellow, 0.5f );
        
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(head.position, stats.AudioRange);
        }

        public void SetStats(HearStats hearingType)
        {
            stats = hearingType;
        }
    }
}
