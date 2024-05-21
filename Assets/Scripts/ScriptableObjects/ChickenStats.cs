using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ChickenChaser/ChickenStats", fileName = "ChickenStats", order = 5)]
    public class ChickenStats : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float maxSpeed;

        public float Speed => speed;
        public float MaxSpeed => maxSpeed;
    
        [Header("Foot Management")] 
        [SerializeField] protected float footRadius;
        [SerializeField] protected float footDistance;

        public float FootRadius => footRadius;
        public float FootDistance => footDistance;
    
        //If there are multiple types of chicken, this should actually probably be seperated.
        //As the sound effects will likely be the same regardless.
        [Header("Effects")] 
        [SerializeField] private AudioClip bounceSfx;
        [SerializeField] private AudioClip bushSfx;
        [SerializeField] private AudioClip waterSfx;
        [SerializeField] private AudioClip onCapture;
        [SerializeField] private AudioClip onEscape;

        public AudioClip BounceSfx => bounceSfx;
        public AudioClip BushSfx => bushSfx;
        public AudioClip WaterSfx => waterSfx;
        public AudioClip OnCapture => onCapture;
        public AudioClip OnEscape => onEscape;
    }
}
