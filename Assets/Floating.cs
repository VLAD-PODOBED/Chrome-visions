using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;     // Амплитуда колебания
    [SerializeField] private float frequency = 1f;       // Частота колебания
    [SerializeField] private float tiltAngle = 5f;      // Максимальный угол наклона
    [SerializeField] private float tiltSpeed = 0.5f;    // Скорость наклона
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    
    void Start()
    {
        // Сохраняем начальную позицию и поворот объекта
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    
    void Update()
    {
        // Вертикальное движение
        float newY = startPosition.y + amplitude * Mathf.Sin(Time.time * frequency);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Наклон объекта
        float tilt = tiltAngle * Mathf.Sin(Time.time * tiltSpeed);
        transform.rotation = startRotation * Quaternion.Euler(tilt, 0f, tilt);
    }
}
