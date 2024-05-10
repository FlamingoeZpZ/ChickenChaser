using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class LossEffects : MonoBehaviour
{
    private void OnEnable()
    {
        CaptureZone.onGameLoss += OnGameEnd;
    }

    private void OnDisable()
    {
        CaptureZone.onGameLoss -= OnGameEnd;
    }

    private void OnGameEnd()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
