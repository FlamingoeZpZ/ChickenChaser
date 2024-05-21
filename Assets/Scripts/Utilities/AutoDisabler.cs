using System.Collections;
using UnityEngine;

public class AutoDisabler : MonoBehaviour
{
    [SerializeField] private float activeTime;

    private void OnEnable()
    {
        StartCoroutine(LifeHandler());
    }

    private IEnumerator LifeHandler()
    {
        yield return new WaitForSeconds(activeTime);
        gameObject.SetActive(false);
    }

}
