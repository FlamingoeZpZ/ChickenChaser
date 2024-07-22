using UnityEngine;

public interface ITrappable 
{
    //This interface exists merely to bridge the complete from what needs to be done code.
    //Additionally, this script implies that anything can be captured as long as it's ITrappable
    //Finally, this is exactly what you'd see when working with games that are modding friendly, for instance in minecraft,
    //where interfaces like "Hopper", and "Enemy" create flexible code

    //Returns our transform, necessary for the trap functionality.
    public Transform GetTransform();
    
    public bool CanBeTrapped();
    
    //What happens when we are captured?
    public void OnCaptured();

    //What happens when we are released?
    public void OnReleased();

    //What happens before capture? (We should probably disable any logic that'd interfere)
    public void OnPreCapture();
}
