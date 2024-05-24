using System;
using Cinemachine;
using UnityEngine;

public class WinEffects : MonoBehaviour
{

    private CinemachineVirtualCamera _cam;
    void OnEnable()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnDisable()
    {
    }

    private void OnGameWon()
    {
        _cam.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
