using System;
using UnityEngine;
using Utilities;

public class EndGoal : MonoBehaviour
{
    //Our action can be static, as we will only have one EndGoal.
    public static Action onGameWon;
    public static Vector3 TargetPosition { get; private set; }
    [SerializeField] private Transform moveToLocation;

    private void Awake()
    {
        //Make sure to reset the old action
        //onGameWon = null; (everything is self responsible)
        TargetPosition = moveToLocation.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if( 1 << other.gameObject.layer == StaticUtilities.PlayerLayer){
            //We won!
            print("Winner");
            onGameWon.Invoke();
        }
    }
}
