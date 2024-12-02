using UnityEngine;

public class ArcadeCarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 1500f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float steeringAngle = 50f;
    [SerializeField] private float brakeForce = 100f;
    [SerializeField] private float gravityForce = 40f;
    [SerializeField] private float initialAccelerationMultiplier = 2f; 
    [SerializeField] private float initialAccelerationDuration = 4f; 

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private Rigidbody rb;
    private bool isInitialAcceleration = true;
    private float initialAccelerationEndTime;
    public bool enabled;

    private void Start()
    {
        enabled = true;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.9f, 0); 
        initialAccelerationEndTime = Time.time + initialAccelerationDuration; 
    }

    private void Update()
    {
        if (!enabled) return;
        
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }

    private void HandleMotor()
    {
        float motorTorque = acceleration * Input.GetAxis("Vertical");
        if (isInitialAcceleration)
        {
            motorTorque *= initialAccelerationMultiplier;
            if (Time.time >= initialAccelerationEndTime)
            {
                isInitialAcceleration = false;
            }
        }

        if (rb.velocity.magnitude < maxSpeed)
        {
            frontLeftWheelCollider.motorTorque = motorTorque;
            frontRightWheelCollider.motorTorque = motorTorque;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyBraking();
        }
        else
        {
            ReleaseBraking();
        }

        rb.AddForce(Vector3.down * gravityForce * rb.mass);
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    private void ReleaseBraking()
    {
        frontLeftWheelCollider.brakeTorque = 0;
        frontRightWheelCollider.brakeTorque = 0;
        rearLeftWheelCollider.brakeTorque = 0;
        rearRightWheelCollider.brakeTorque = 0;
    }

    private void HandleSteering()
    {
        float steer = steeringAngle * Input.GetAxis("Horizontal");
        frontLeftWheelCollider.steerAngle = steer;
        frontRightWheelCollider.steerAngle = steer;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    public void StopCar()
    {
        if (rb != null)
        {
            // Мгновенно останавливаем машину
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Применяем тормоза
            ApplyBraking();
        }
    }
}
