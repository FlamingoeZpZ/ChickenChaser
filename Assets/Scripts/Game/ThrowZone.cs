using System;
using Characters;
using UnityEngine;
using Utilities;

/// <summary>
/// This object is thrown by the human, and just simulates trajectory so that the cages don't go through the floor.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ThrowZone : MonoBehaviour
{
    [SerializeField] private ChickenTrapLander trapPrefab;
    [SerializeField] private ParticleSystem onLandParticle;

    private const float MinSpeed = 0.1f;

    private const float SpawnRadiusCheck = 0.5f;
    private const float SpawnHeight = 15;
    private Rigidbody _rb;
    private Chicken _caught;

    private float lifeTime;

    // Update is called once per frame
    void FixedUpdate()
    {
        float speed = _rb.velocity.sqrMagnitude;
        //If we reach this point, we have achieved our goal
        lifeTime += Time.fixedDeltaTime;
        if (lifeTime > 0.5f && speed < MinSpeed)
        {
            print("Activating ThrowZone : " + speed);
            Vector3 startPoint;
            Vector3 velocity = Vector3.down;
            if (Physics.SphereCast(transform.position, SpawnRadiusCheck, Vector3.up, out RaycastHit hit, SpawnHeight))
            {
                startPoint = transform.position + Vector3.up * hit.distance;
                velocity *= hit.distance;
            }
            else
            {
                startPoint = transform.position + SpawnHeight * Vector3.up;
                velocity *= SpawnHeight;
            }
            
            //_collider.enabled = true;

            //Spawn in a cage object directly above us
            ChickenTrapLander trap = Instantiate(trapPrefab, startPoint, Quaternion.identity);

            trap.Initialize(velocity, _caught);
            
            //Leave our child parentless temporarily.
            transform.GetChild(0).SetParent(null, true);

            ParticleSystem ps = Instantiate(onLandParticle, transform.position, Quaternion.LookRotation(transform.up));
            ps.Play();
            Destroy(ps.gameObject ,3);

            //Destroy ourselves, we've served our purpose.
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 force, Chicken caught)
    {
        _caught = caught;
        caught.transform.parent = transform;
        caught.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        _rb = GetComponent<Rigidbody>();
        
  
        _rb.velocity = force;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        GizmosExtras.DrawWireSphereCast(transform.position, Vector3.up, SpawnHeight, SpawnRadiusCheck);
    }

    
}
