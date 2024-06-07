using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class IntroDirector : MonoBehaviour
{
    [SerializeField] private AnimationCurve timeDilation;
    [SerializeField] private ParticleSystem[] particles;
    private PlayableDirector _director;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        _director.Play();
        double needed = _director.duration;
        double actual = 0;
        while (actual < needed)
        {
            float eval = timeDilation.Evaluate((float)actual / (float)needed);
            actual += eval * Time.deltaTime;
            _director.time = actual;
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.simulationSpeed = eval;
            }

            yield return null;
        }
        _director.time = needed;
        _director.Stop();
    }
}
