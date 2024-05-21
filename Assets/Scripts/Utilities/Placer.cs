using UnityEngine;

public class Placer : MonoBehaviour
{
    [SerializeField] private Collider prefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 initialOffset;

    [SerializeField, Min(1)] private int numX = 1;
    [SerializeField, Min(0)] private int numY = 0;
    [SerializeField, Min(0)] private int numZ = 0;

    [SerializeField] private bool applyOffsetX = true;
    [SerializeField] private bool applyOffsetY;
    [SerializeField] private bool applyOffsetZ = true;

    private int tNumX;
    private int tNumY;
    private int tNumZ;
    private Vector3 tOffset;
    private Vector3 tOnitialOffset;
    private Vector3 scalar;


    private Vector3 halfBounds;
    private void OnEnable()
    {
       
        ReplacePropsInRow();
        
    }


    [ContextMenu("PlaceProps")]
    void ReplacePropsInRow()
    {
        scalar = new Vector3(applyOffsetX ? 1 : 0, applyOffsetY ? 1 : 0, applyOffsetZ ? 1:0);
        tNumX = numX;
        tNumY = numY;
        tNumZ = numZ;
        tOffset = offset;
        tOnitialOffset = initialOffset;
        //Destroy all children;
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        //Cache
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        Quaternion rotation = Quaternion.identity;
        
        //Spawn
        Collider c = Instantiate(prefab, position, rotation, transform1);
        halfBounds = c.bounds.size;
        Vector3 localOffset = position - Vector3.Scale(halfBounds/2,scalar);
        localOffset.x += initialOffset.x;
        
        c.transform.position = localOffset;
        for (int i = 1; i < numX; ++i)
        {
            localOffset.x += (halfBounds.x + offset.x);
            Instantiate(prefab, localOffset, rotation, transform1);
        }
        
       

        localOffset = position- Vector3.Scale(halfBounds/2,scalar);
        localOffset.y += initialOffset.y;
        rotation = Quaternion.Euler(90,0,0);
        for (int i = 0; i < numY; ++i)
        {
            localOffset.y += (halfBounds.x + offset.x);
            Instantiate(prefab, localOffset, rotation, transform1);
        }
        
        
        
        localOffset = position- Vector3.Scale(halfBounds/2,scalar);
        localOffset.z += initialOffset.z;
        rotation = Quaternion.Euler(0,90,0);
        for (int i = 0; i < numZ; ++i)
        {
            localOffset.z += (halfBounds.x + offset.x);
            Instantiate(prefab, localOffset, rotation, transform1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (tNumX != numX || tNumY != numY || tNumZ != numZ || tOffset != offset || tOnitialOffset != initialOffset) ReplacePropsInRow();
    }
}
