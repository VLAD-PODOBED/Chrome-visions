using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngile = 45f;
    public float turnSpeed = 0.8f;
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 150f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public bool isBraking = false;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer;

    [Header("Sensors")]
    public float sensorLength = 1f;
    public Vector3 frontSensorPosition = new Vector3(0f,0.07f,0.2f);
    public float frontSideSensorPosition = 0.08f;
    public float frontSensorAngle = 30;

    private List<Transform> nodes;
    private int currentNode = 0;
    private bool avoiding = false;
    private float targerSteerAngle = 0;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
                nodes.Add(pathTransforms[i]);
        }
    }


    private void FixedUpdate()
    {
        Sensors();
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        Braking();
        LerpToSteerAngle();
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartingPos = transform.position;
        sensorStartingPos += transform.forward * frontSensorPosition.z;
        sensorStartingPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;

        //front right sensor
        sensorStartingPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartingPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f; 
            } 
        }


        //front right angle sensor
        else if (Physics.Raycast(sensorStartingPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }


        //front left sensor
        sensorStartingPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartingPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }


        //front left angle sensor
        else if (Physics.Raycast(sensorStartingPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartingPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }

        //front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartingPos, transform.forward, out hit, sensorLength))
            {
                if (!hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorStartingPos, hit.point);
                    avoiding = true;
                    if (hit.normal.z < 0)
                    {
                        avoidMultiplier = 1;
                    }
                    else
                    {
                        avoidMultiplier = -1;
                    }
                }
            }
        }
        if (avoiding)
        {
            targerSteerAngle = maxSteerAngile * avoidMultiplier;
        }
    }
    private void ApplySteer()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngile;
        targerSteerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * frontLeftWheel.radius * frontLeftWheel.rpm * 60 / 1000;

        if(currentSpeed < maxSpeed &&!isBraking)
        {
            frontLeftWheel.motorTorque = maxMotorTorque;
            frontRightWheel.motorTorque = maxMotorTorque;
        }
        else
        {
            frontLeftWheel.motorTorque = 0;
            frontRightWheel.motorTorque = 0;
        }
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else currentNode++;
    }


    private void Braking()
    {
        if (isBraking)
        {
            carRenderer.material.mainTexture = textureBraking;
            rearLeftWheel.brakeTorque = maxBrakeTorque;
            rearRightWheel.brakeTorque = maxBrakeTorque;
        }
        else
        {
            carRenderer.material.mainTexture = textureNormal;
            rearLeftWheel.brakeTorque = 0;
            rearRightWheel.brakeTorque = 0;
        }
    }

    private void LerpToSteerAngle()
    {
        frontLeftWheel.steerAngle = Mathf.Lerp(frontLeftWheel.steerAngle, targerSteerAngle, Time.deltaTime * turnSpeed);
        frontRightWheel.steerAngle = Mathf.Lerp(frontRightWheel.steerAngle, targerSteerAngle, Time.deltaTime * turnSpeed);
    }
}