using Characters;
using UnityEngine;

public class EndGoal : MonoBehaviour
{
    //Our action can be static, as we will only have one EndGoal.
    //public static Action onGameWon;
    [SerializeField] private Transform moveToLocation;
    [SerializeField] private LayerMask allowedLayers;

    void OnTriggerEnter(Collider other)
    {
        //If they're not a desired layer.
        if ((other.gameObject.layer & allowedLayers) == 0) return;
        
        if (other.attachedRigidbody.TryGetComponent(out Chicken c))
        {
            c.EscapeAndMoveTo(moveToLocation.position);
        }
    }
}
