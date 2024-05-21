using UI;
using UnityEngine;
using Utilities;

namespace Ability
{
    public class Cluck : AbilityBase
    {

        [SerializeField] private ParticleSystem cluckParticle;
        [SerializeField] private AudioClip cluckSound;
        [SerializeField] private float minDelay;
        [SerializeField] private float maxDelay;

        private AudioSource _source;
        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public override int AbilityNum()
        {
            return StaticUtilities.CluckAnimID;
        }

        protected override void Activate()
        {
            cluckParticle.Play();
            _source.pitch = Random.Range(0.8f, 1.2f);
            _source.PlayOneShot(cluckSound, SettingsManager.currentSettings.SoundVolume * 1);
            AudioManager.onSoundPlayed.Invoke(transform.position, 1, 20);
        }
    }
}
