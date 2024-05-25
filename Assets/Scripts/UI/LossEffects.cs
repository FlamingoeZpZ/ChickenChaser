using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Game;
using UnityEngine;

public class LossEffects : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerChicken.onPlayerCaught += OnGameEnd;
    }

    private void OnDisable()
    {
        PlayerChicken.onPlayerCaught -= OnGameEnd;
    }

    private void OnGameEnd(Vector3 _)
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
