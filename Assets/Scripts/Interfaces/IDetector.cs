using System;
using UnityEngine;

namespace Interfaces
{
    public interface IDetector
    {
        public float GetDetection();
        public void AddDetection(Vector3 location,float detection, EDetectionType type);
    }

    [Flags]
    public enum EDetectionType
    {
        Visual = 1,
        Audio = 2
    }
}
