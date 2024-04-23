using System;
using UnityEngine;

[RequireComponent(typeof(Dash))]
public class Chicken : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;

    [Header("Looking")] 
    [SerializeField , Range(0,90)] private float pitchLimit;
    [SerializeField, Range(0,180)] private float yawLimit;
    [SerializeField, Range(0.01f,5)] private float yawSpeed;
    [SerializeField, Range(0.01f,5)] private float pitchSpeed;
    [SerializeField] private Transform head;


    
    
    private Vector3 _moveDirection;
    private Vector2 _lookDirection;
    private Rigidbody _rb;
    private Dash _dashController;
    
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _dashController = GetComponent<Dash>();
        PlayerControls.Init(this);
        PlayerControls.DisableUI();
        _rb.maxLinearVelocity = maxSpeed;
    }
    

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rb.AddForce(transform.rotation * _moveDirection * speed);
    }

    private void LateUpdate()
    {
        HandleLooking();
    }

    private void HandleLooking()
    {
        float timeShift = Time.deltaTime;
        float pitchChange = head.localEulerAngles.x - pitchSpeed * _lookDirection.y * timeShift;
        float yawChange = transform.localEulerAngles.y + yawSpeed * _lookDirection.x * timeShift;
        
        if (pitchChange > pitchLimit && pitchChange < 180) pitchChange = pitchLimit;
        else if (pitchChange < 360-pitchLimit && pitchChange > 180) pitchChange = -pitchLimit;
        if (yawChange > yawLimit && yawChange < 180) yawChange = yawLimit;
        else if (yawChange < 360-yawLimit && yawChange > 180) yawChange = -yawLimit;

        transform.localEulerAngles = new Vector3(0, yawChange, 0);
        head.localEulerAngles = new Vector3(pitchChange, 0, 0);
    }

    public void TryCluck()
    {
        print("Trying Cluck");
    }

    public void TryDash()
    {
        print("Trying Dash");
        _dashController.TryDash(head.forward);
    }

    
    public void Look(Vector2 readValue)
    {
        _lookDirection = readValue;
    }

    public void Move(Vector2 readValue)
    {
        _moveDirection = new Vector3(readValue.x, 0, readValue.y);
    }
}
