using AI;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Ability
{
    public class Cluck : AbilityBase
    {

        [SerializeField] private ParticleSystem cluckParticle;
        [SerializeField] private AudioClip cluckSound;

        private const float AudioScalar = 0.3f;
        private AudioSource _source;
        private void Awake()
        {
            _source = GetComponentInChildren<AudioSource>();
        }

        protected override void Activate()
        {
            cluckParticle.Play();
            _source.pitch = Random.Range(0.8f, 1.2f);
            _source.PlayOneShot(cluckSound, SettingsManager.currentSettings.SoundVolume * AudioScalar);
            AudioDetection.onSoundPlayed.Invoke(transform.position, 10, 20, EAudioLayer.ChickenEmergency);
        }

        public override bool CanActivate()
        {
            //Must be moving, or barely.
            return Owner.MoveSpeed < 0.2f && base.CanActivate();
        }

        protected override int AbilityTriggerID() => StaticUtilities.JumpAnimID;
        protected override int AbilityBoolID() => StaticUtilities.CluckAnimID;
    }
}
