using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GroupEnabler : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private float delay;
        [SerializeField] private bool delayFirst;
    
        // Start is called before the first frame update
        void OnEnable()
        {
            StartCoroutine(Activate());
        }

        private IEnumerator Activate()
        {
            //QOL
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            if(delayFirst) yield return new WaitForSeconds(delay);
            for (int i = 0; i < transform.childCount; ++i)
            {
                
                transform.GetChild(i).gameObject.SetActive(true);
                yield return new WaitForSeconds(delay);
            }
            if(startButton) startButton.Select();
        }
    }
}
