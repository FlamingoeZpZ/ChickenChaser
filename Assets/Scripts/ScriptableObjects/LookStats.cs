using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "LookStats", menuName = "ChickenChaser/LookStats", order = 2)]
    public class LookStats : ScriptableObject
    {
        [Header("AI | Vision Cone")] 
        [SerializeField, Range(-1, 1)]  private float visionFOV;
        [SerializeField, Min(0)] private float visionDistance;
        [SerializeField, Min(0)] private float sightDetectionValue = 2;
        [SerializeField] private AnimationCurve sightDetectionDropOff;

        public float VisionFOV => visionFOV;
        public float VisionDistance => visionDistance;
        public float SightDetectionValue(float distance) => sightDetectionDropOff.Evaluate(distance) * sightDetectionValue;
    }
}
