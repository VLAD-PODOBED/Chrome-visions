using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float distanceTravelled;

    void Update()
    {
        distanceTravelled += Time.deltaTime * GetComponent<Rigidbody>().velocity.magnitude;
    }

    public float GetDistanceTravelled()
    {
        return distanceTravelled;
    }
}