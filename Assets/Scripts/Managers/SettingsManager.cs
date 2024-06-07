using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class SettingsManager
{
    private static readonly string Directory = Application.persistentDataPath + "/Settings.dat";
    public static SaveFile currentSettings;

    static SettingsManager()
    {
        // Load the data when the game begins
        LoadData();
        QualitySettings.SetQualityLevel((int)currentSettings.graphics);
    }

    public static async void LoadData()
    {
        try
        {
            if (File.Exists(Directory))
            {
                await using FileStream sr = File.OpenRead(Directory);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveFile));
                currentSettings = (SaveFile)xmlSerializer.Deserialize(sr);
            }
            else
            {
                // Initialize with default settings if the file doesn't exist
                ResetSettings();
            }
        }
        catch (Exception)
        {
            ResetSettings();
            SaveData();
        }
    }

    public static void ResetSettings()
    {
        currentSettings = new SaveFile
        {
            LookSensitivity = 1.0f,
            MusicVolume = 0.5f,
            SoundVolume = 0.5f,
            UIScale = 0.5f,
            graphics = EGraphicsState.Balanced,
            fullScreen = FullScreenMode.FullScreenWindow
        };
    }

    public static async void SaveData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));

        await using TextWriter writer = new StreamWriter(Directory);
        serializer.Serialize(writer, currentSettings);
    }

    public enum EGraphicsState
    {
        Performant,
        Balanced,
        HighFidelity
    }

    [Serializable]
    public struct SaveFile
    {
        private float _lookSens;
        private float _musicVolume;
        private float _soundVolume;
        private float _uiScale;

        //No callbacks needed
        public EGraphicsState graphics;
        public FullScreenMode fullScreen;

        public static Action<float> onLookSenseChanged = null;
        public static Action<float> onMusicVolumeChanged = null;
        public static Action<float> onSoundVolumeChanged = null;
        public static Action<float> onUIScaleChanged = null;
        
        public float LookSensitivity
        {
            get => _lookSens;
            set
            {
                _lookSens = value;
                onLookSenseChanged?.Invoke(value);
            }
        }
        
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                onMusicVolumeChanged?.Invoke(value);
            }
        }
        
        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = value;
                onSoundVolumeChanged?.Invoke(value);
            }
        }
        
        
        public float UIScale
        {
            get => _uiScale;
            set
            {
                _uiScale = value;
                onUIScaleChanged?.Invoke(value);
            }
        }

        
    }
}
