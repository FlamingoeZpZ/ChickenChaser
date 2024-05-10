using Interfaces;
using UnityEngine;

namespace ScriptableObjects
{
  [CreateAssetMenu(fileName = "AIStats", menuName = "ChickenChaser/AIStats", order = 1)]
  public class AiStats : ScriptableObject
  {
  
    [Header("AI|Decay")]
    [SerializeField] private float maxDetection = 100;
    [SerializeField] private float detectionDecayRate = 5;
    [SerializeField] private float beginDecayCooldown = 3;
    [SerializeField, Min(0)] private float lookingStateDetectionModifier = 2;
    [SerializeField, Min(0)] private float idleStateDetectionModifier = 1;
    [SerializeField, Min(0)] private float baseMoveSpeed = 2.5f;
    [SerializeField, Min(0)] private float chaseMoveSpeed = 4;
    [SerializeField] private EDetectionType ignoreWhileChasing;
  
    public float MaxDetection => maxDetection;
    public float DetectionDecayRate => detectionDecayRate;
    public float BeginDecayCooldown => beginDecayCooldown;
    public float LookingStateDetectionModifier => lookingStateDetectionModifier;
    public float IdleStateDetectionModifier => idleStateDetectionModifier;
    public float BaseMoveSpeed => baseMoveSpeed;
    public float ChaseMoveSpeed => chaseMoveSpeed;

    public EDetectionType IgnoreWhileChasing => ignoreWhileChasing;
    
  
    [SerializeField, Min(0)] private float minIdleTime = 1;
    [SerializeField, Min(0)] private float maxIdleTime = 3;
  
    public float MinIdleTime => minIdleTime;
    public float MaxIdleTime => maxIdleTime;
  


    
  }
}
