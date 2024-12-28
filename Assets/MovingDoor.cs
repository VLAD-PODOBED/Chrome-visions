using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour
{
    private float targetAngle = 120f;
    private float duration = 4f;
    private float elapsedTime = 0f;
    private bool isOpening = false;

    public void OpenDoor()
    {
        isOpening = true;
    }

    void Update()
    {
        if (isOpening)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(0, targetAngle, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(-90, 0, angle);
            
            if (elapsedTime >= duration)
            {
                elapsedTime = duration;
                isOpening = false;
            }
        }
    }
}
