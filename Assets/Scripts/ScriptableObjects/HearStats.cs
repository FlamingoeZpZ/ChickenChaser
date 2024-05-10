using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "HearStats", menuName = "ChickenChaser/HearStats", order = 3)]
    public class HearStats : ScriptableObject
    {
        [Header("AI | Audio")] 
        [SerializeField, Min(0)]private float audioRange;
        [SerializeField, Min(0)] private float audioDetectionValue = 1;
        [SerializeField] private AnimationCurve audioDetectionDropOff;
        public float AudioRange => audioRange;
        public float AudioDetectionValue(float distance) => audioDetectionDropOff.Evaluate(distance) * audioDetectionValue;
    }
}
