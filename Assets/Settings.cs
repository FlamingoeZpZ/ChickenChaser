using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Selectable startingSelectable;

    [SerializeField] private Slider[] sliders;
    
    private void OnEnable()
    {
        startingSelectable.Select();
    }

    private void Start()
    {
        //Apply Loaded Settings
        
    }

    public void ResetSaveData()
    {
        
    }

    public void SaveData()
    {
        
    }

    private void OnDisable()
    {
        print("TODO: If the game has started");
        SaveData();
        if (false)
        {
            PlayerControls.DisableUI();
        }
    }
}
