using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private bool _inGame;
    
        private static GameManager Instance;
    
        public static bool InGame => Instance._inGame;

        [SerializeField] private float closeTime;
        [SerializeField] private float openTime;
        [SerializeField] private AnimationCurve closeCurve;
        [SerializeField] private AnimationCurve openCurve;

        [SerializeField] private float intendedDelay = 1;
    
        [SerializeField] private GameObject canvas;
        //Honestly, this should be in another script, but I cant be bothered.
        [Header("Sounds")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip roundStart;
        [SerializeField] private AudioClip stealthMusic;
        [SerializeField] private AudioClip chaseMusic;
        [SerializeField] private AudioClip mainMusic;

        [SerializeField] private AudioVolumeRangeSet[] audioSets;

        public static readonly Dictionary<string, AudioVolumeRangeSet> SoundsDictionary =
            new Dictionary<string, AudioVolumeRangeSet>();
        
        private GameObject _textBlocks;
        private Material _transition;
        

        private bool _initialized;
        private const float StartTime = 6.4f;
    
        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(Instance.gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;

            _transition = canvas.transform.GetChild(0).GetComponent<Image>().material;
            _textBlocks = canvas.transform.GetChild(0).GetChild(0).gameObject;

            source.time = StartTime;
            
            StartCoroutine( TransitionScreen(closeTime, closeCurve, false));

            SettingsManager.SaveFile.onMusicVolumeChanged += x =>
            {
                source.volume = x;
            };

            SoundsDictionary.Clear();
            foreach (var set in audioSets)
            {
                SoundsDictionary.Add(set.tag, set);
            }
        }

        public static void PlayUISound(AudioClip clip)
        {
            Debug.Log("Callstrac");
            Instance.source.PlayOneShot(clip, SettingsManager.currentSettings.SoundVolume);
        }


        private IEnumerator LoadGameImpl()
        {
        
            _inGame = true;
        
            //OStartCoroutine(PEN UI COVERING
            yield return TransitionScreen(openTime, openCurve, true);
            yield return new WaitForSeconds(openTime);
            source.Stop();
            DateTime currentTime = DateTime.Now;
            //Additively remove the player scene?
            SceneManager.UnloadSceneAsync(2).completed += _ =>
                //Additively load the main menu
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive).completed += _ =>
                    StartCoroutine(ReadyGame(currentTime));
            
        }

        private IEnumerator ReadyGame(DateTime startTime)
        {
            var timeSpan = DateTime.Now.Subtract(startTime);
            float s = intendedDelay-timeSpan.Seconds;
            if (s > 0) yield return new WaitForSeconds(s);
            yield return StartCoroutine( TransitionScreen(closeTime, closeCurve, false));

            if (_inGame)
            {
                source.PlayOneShot(roundStart, SettingsManager.currentSettings.SoundVolume);
                source.clip = stealthMusic;
            }
            else
            {
                source.clip = mainMusic;
                source.time = StartTime;
            }
            source.Play();

        }

        private IEnumerator LoadMenuImpl()
        {
            _inGame = false;
            //OStartCoroutine(PEN UI COVERING
            yield return TransitionScreen(openTime, openCurve, true);
            yield return new WaitForSeconds(openTime);
            source.Stop();

            DateTime currentTime = DateTime.Now;

            //Additively remove the player scene?
            SceneManager.UnloadSceneAsync(1).completed += _ =>
                //Additively load the main menu
                SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive).completed += _ =>
                    StartCoroutine(ReadyGame(currentTime));
            //CLOSE UI COVERING
        }

        public static void LoadGame()
        {
            //Instance.LoadGameImpl();
            Instance.StartCoroutine(Instance.LoadGameImpl());
        }

        public static void LoadMainMenu()
        {
            Instance.StartCoroutine(Instance.LoadMenuImpl());
        }

        private IEnumerator TransitionScreen(float duration, AnimationCurve curve, bool isOpen)
        {
            float o = 0;
        
            canvas.SetActive(true);
            if (!isOpen)
            {
                _textBlocks.SetActive(false);
            }

            _transition.SetFloat(StaticUtilities.FillMatID, curve.Evaluate(0));
            float audioMin = SettingsManager.currentSettings.MusicVolume;
            while (o < duration)
            {
                o += Time.deltaTime;
                float perc = o / duration;
                float eval = curve.Evaluate(perc);
                source.volume = Mathf.Min(audioMin,eval);
                _transition.SetFloat(StaticUtilities.FillMatID, eval);
                yield return null;
            }

            if (!isOpen)
            {
                canvas.SetActive(false);
            }
            else
            {
                _textBlocks.SetActive(true);
            }

            _transition.SetFloat(StaticUtilities.FillMatID, curve.Evaluate(1));
        }


        public static void TransitionGameMusic(bool isChasing, float duration)
        {
            Instance.StartCoroutine(Instance.source.TransitionSound(isChasing?Instance.chaseMusic:Instance.stealthMusic,duration));
        }

        
    }

    [Serializable]
    public struct AudioVolumeRangeSet
    {
        public AudioClip clip;
        [Range(0,1)]public float volume;
        [Min(0)]public float rangeMultiplier;
        public string tag; // Alternative (which would be for the best) is to make a custom editor... and That's not happenening, atleast not right now.
    }
    

}
