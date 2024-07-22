using System.Collections;
using Characters;
using Game;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(Rigidbody))]
public class ChickenTrapLander : MonoBehaviour
{
    private const float SpawnSpeed = 1.2f; // percent based on starting height. a start of 15 at 70% means the starting speed is 15 * 0.7
    private Rigidbody _rb;
    private ITrappable _caught;

    [SerializeField] private Transform startPoint;
    [SerializeField] private float distance;
    [SerializeField] private ParticleSystem onLandParticle;

    private static readonly Vector3 offset = new Vector3(0, 0.4f, 0);
    // Update is called once per frame
    void FixedUpdate()
    {
        //We're already moving straight down, all we need to know is if we hit a ground layer

        if (Physics.Raycast(startPoint.position, Vector3.down, out RaycastHit hit, distance,
                StaticUtilities.GroundLayers))
        {
            transform.position = hit.point + offset;
            //Then we need to unparent our child
            Transform cage = transform.GetChild(0);
            cage.SetParent(null, true);
            
            //And parent our chicken to the cage. This can probably be improved, but it'd be hard to notice regardless.
            cage.GetComponentInChildren<ChickenTrap>().AttachChicken(_caught);
            
            ParticleSystem ps = Instantiate(onLandParticle, hit.point, Quaternion.LookRotation(transform.up));
            ps.Play();
            Destroy(ps.gameObject ,3);
            //Finally, we need to destroy ourselves as we've served our purpose
            Destroy(gameObject);
        }

    }


    // ReSharper disable Unity.PerformanceAnalysis
    public void Initialize(Vector3 velocity, ITrappable getChild)
    {
        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = velocity * SpawnSpeed;
        _caught = getChild;
        //Prevent sillies
        //FAIL SAFE, expect the object  we're expecting to cage will now just exist disabled forever.
        StartCoroutine(Emergency());

    }

    private IEnumerator Emergency()
    {
        yield return new WaitForSeconds(3);
        if (!isActiveAndEnabled) yield break;
        Destroy(gameObject);
        _caught.OnReleased();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(startPoint.position, Vector3.down* distance);
    }
}
