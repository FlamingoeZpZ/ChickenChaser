
using UnityEngine;

namespace Utilities
{
    public static class GizmosExtras
    {
        public static void DrawWireSphereCast(Vector3 start, Vector3 direction, float distance, float radius)
        {
            Gizmos.DrawWireSphere(start, radius);
            Gizmos.DrawRay(start, direction * distance);
            Gizmos.DrawWireSphere(start + direction * distance, radius);
        }
    }
}
