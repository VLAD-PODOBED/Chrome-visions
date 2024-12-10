using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour
{
    private float targetAngle = 120f;
    private float duration = 4f;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        float angle = Mathf.Lerp(0, targetAngle, elapsedTime / duration);
        transform.rotation = Quaternion.Euler(-90, 0, angle);
        
        // Остановить вращение после достижения целевого угла
        if (elapsedTime >= duration)
        {
            elapsedTime = duration; // Зафиксировать время
        }
    }
}
