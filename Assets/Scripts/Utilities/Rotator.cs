using UnityEngine;

public class Rotator : MonoBehaviour
{
        [SerializeField] private float rotationSpeed;
        private void LateUpdate()
        {
                transform.Rotate(Vector3.up,  rotationSpeed * Time.deltaTime, Space.World);
        }
}
