using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [Header("Dash")] 
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private float _dashCheckRadius;

    private Rigidbody _rb;
    private bool _canDash = true;
    private float _radius;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _radius = GetComponentInChildren<SphereCollider>().radius;
        _dashCheckRadius = _radius / 2;
    }

    public void TryDash(Vector3 direction)
    {
        //can I dash?
        if (!_canDash) return;
        StartCoroutine(DashHandler(direction));
    }

    private IEnumerator DashHandler(Vector3 direction)
    {
        _canDash = false;
        Vector3 endPoint =
            Physics.SphereCast(transform.position, _dashCheckRadius, direction, out RaycastHit hit, dashDistance, StaticUtilities.VisibilityLayer)
                ?  hit.point - hit.normal * _radius
                : direction * dashDistance + transform.position;

        float curTime = 0;
        _rb.isKinematic = true;
        while (curTime < dashDuration) 
        {
            curTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, endPoint, curTime/dashDuration);
            yield return null;
        }
        transform.position = Vector3.Lerp(transform.position, endPoint, 1);
        _rb.isKinematic = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

}
