using System;
using UnityEngine;

public class ChickenAnimatorReciever : MonoBehaviour
{

    public Action<float> OnLandEffect = null;
   
    
    //Let's also check if we're inside a volume maker,


    //Magically called via animator
    private void LandEffect(float force)
    {
        OnLandEffect.Invoke(force);
    }    
}
