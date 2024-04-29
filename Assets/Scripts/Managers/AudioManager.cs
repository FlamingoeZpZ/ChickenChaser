using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static Action<Vector3, float> onSoundPlayed;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlaySound(AudioClip sound, Vector3 location, float volume, float range)
    {
        onSoundPlayed?.Invoke(location, volume * range);   
        AudioSource.PlayClipAtPoint(sound, location, volume);
    }

}
