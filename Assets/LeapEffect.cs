using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    
    public void Jump()
    {
        ps.Play();
    }
}
