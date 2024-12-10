using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recordcamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        transform.Rotate(0, 1 * Time.deltaTime, 0); // Поворачиваем камеру вправо
    }

}
