using System;
using Cinemachine;
using UnityEngine;

public class WinEffects : MonoBehaviour
{

    private CinemachineVirtualCamera _cam;
    void OnEnable()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        EndGoal.onGameWon += OnGameWon;
    }

    private void OnDisable()
    {
        EndGoal.onGameWon -= OnGameWon;
    }

    private void OnGameWon()
    {
        _cam.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
