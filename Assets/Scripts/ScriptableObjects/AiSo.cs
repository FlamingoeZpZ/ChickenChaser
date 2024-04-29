using UnityEngine;

[CreateAssetMenu(fileName = "AIStats", menuName = "ChickenChaser/AIStats", order = 1)]
public class AiSo : ScriptableObject
{
  
  [Header("AI|Decay")]
  [SerializeField] private float maxDetection = 100;
  [SerializeField] private float detectionDecayRate = 5;
  [SerializeField] private float beginDecayCooldown = 3;
  
  public float MaxDetection => maxDetection;
  public float DetectionDecayRate => detectionDecayRate;
  public float BeginDecayCooldown => beginDecayCooldown;
  
  [SerializeField, Min(0)] private float minIdleTime = 1;
  [SerializeField, Min(0)] private float maxIdleTime = 3;
  
  public float MinIdleTime => minIdleTime;
  public float MaxIdleTime => maxIdleTime;
  
  [Header("AI | Vision Cone")] 
  [SerializeField, Range(-1, 1)]  private float visionFOV;
  [SerializeField, Min(0)] private float visionDistance;
  [SerializeField, Min(0)] private float sightDetectionValue = 2;
  [SerializeField] private AnimationCurve sightDetectionDropOff;

  public float VisionFOV => visionFOV;
  public float VisionDistance => visionDistance;
  public float SightDetectionValue => sightDetectionValue;
  public AnimationCurve SightDetectionDropOff => sightDetectionDropOff;

  [Header("AI | Audio")] 
  [SerializeField, Min(0)]private float audioRange;
  [SerializeField, Min(0)] private float audioDetectionValue = 1;
  [SerializeField] private AnimationCurve audioDetectionDropOff;
 
  public float AudioRange => audioRange;
  public float AudioDetectionValue => audioDetectionValue;
  public AnimationCurve AudioDetectionDropOff => audioDetectionDropOff;
}
