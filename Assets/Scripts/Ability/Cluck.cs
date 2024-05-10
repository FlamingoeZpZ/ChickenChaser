
using System.Collections;
using UnityEngine;

namespace Ability
{
    public class Cluck : AbilityBase
    {

        [SerializeField] private ParticleSystem cluckParticle;
        [SerializeField] private AudioClip cluckSound;
        [SerializeField] private float minDelay;
        [SerializeField] private float maxDelay;
        [SerializeField,Min(0)] private int minClucks;
        [SerializeField,Min(0)] private int maxClucks;


        protected override float AbilityNum()
        {
            return 1;
        }

        protected override void Activate()
        {
            //We need to synchronize our sounds, let's start a coroutine timer
            StartCoroutine(Clucker());
        }

        private IEnumerator Clucker()
        {
            
            int numClucks = Random.Range(minClucks, maxClucks);
            while (numClucks-- >= 0)
            {
                cluckParticle.Play();
                AudioManager.Instance.PlaySound(cluckSound, transform.position, 1, 20);
                yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            }
        }
    }
}
