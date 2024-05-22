using UnityEngine;

namespace Managers
{
    public class AIManager : MonoBehaviour
    {
        private static int _numChasing;

        private void Awake()
        {
            _numChasing = 0;
        }

        public static void BeginChasing()
        {
            if (_numChasing++ == 0)
                GameManager.TransitionGameMusic(true,0.5f);

        }

        public static void StopChasing()
        {
            if (--_numChasing == 0)
                GameManager.TransitionGameMusic(false, 0.5f);
        }


    }
}
