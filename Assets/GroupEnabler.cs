using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupEnabler : MonoBehaviour
{
    [SerializeField] private float delay;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //QOL
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(delay);
        }
        transform.GetChild(0).GetComponent<Selectable>().Select();
    }
}
