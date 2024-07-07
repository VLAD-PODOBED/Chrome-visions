using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class AiCarController : MonoBehaviour
{
    public Transform waypointsParent; // Родительский объект, содержащий все вейпоинты
    public float maxSpeed = 35f;
    public float rotationSpeed = 5f;
    public float slowDownDistance = 5f; // Расстояние, при котором начнется замедление перед вейпоинтом
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private BoxCollider boxCollider;

    // Добавьте ссылки на объекты колес
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    // Скорость вращения колес
    public float wheelRotationSpeed = 1000f;

    // Список для хранения вейпоинтов
    private List<Transform> waypoints = new List<Transform>();

    // Время для набора максимальной скорости
    public float accelerationTime = 5f;
    private float currentSpeed = 0f;
    private float acceleration;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();

        agent.updatePosition = true; // Управление позицией через NavMeshAgent
        agent.updateRotation = false; // Мы будем управлять вращением машинки вручную

        agent.speed = maxSpeed;
        acceleration = maxSpeed / accelerationTime;

        // Если задан родительский объект вейпоинтов, добавляем их
        if (waypointsParent != null)
        {
            AddWaypointsFromParent();
        }

        if (waypoints.Count > 0)
        {
            SetNextDestination();
        }

        // Настройка размеров NavMeshAgent
        AdjustNavMeshAgentSize();
    }

    void Update()
    {
        if (waypoints.Count == 0) return;

        // Если агент достиг текущего пункта назначения
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            SetNextDestination();
        }

        // Управление движением через Rigidbody
        MoveCar();
    }

    void SetNextDestination()
    {
        // На случай, если waypoints стали пустыми, выходим из метода
        if (waypoints.Count == 0) return;

        // Находим индекс следующей точки
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;

        // Устанавливаем следующую точку назначения
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void MoveCar()
    {
        // Получаем направление к следующему вейпоинту
        Vector3 direction = (agent.steeringTarget - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, agent.steeringTarget);

        // Плавное замедление при приближении к вейпоинту
        float adjustedSpeed = Mathf.Min(currentSpeed, maxSpeed);
        if (distance < slowDownDistance)
        {
            adjustedSpeed = Mathf.Lerp(0, adjustedSpeed, distance / slowDownDistance);
        }

        // Проверяем, поворачивает ли машина и замедляем на 30-40%
        if (IsTurning())
        {
            adjustedSpeed *= 0.6f; // Замедляем на 40%
        }

        // Управляем движением Rigidbody
        rb.velocity = direction * adjustedSpeed;

        // Плавное вращение машинки в сторону движения
        if (rb.velocity.magnitude > 0.1f) // Вращаем только если есть движение вперед
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }

        // Вращаем колеса
        RotateWheels();

        // Позиция машинки равна позиции NavMeshAgent
        transform.position = agent.transform.position;

        // Увеличиваем скорость до максимальной в течение первых нескольких секунд
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
    }

    bool IsWaypointVisible()
    {
        RaycastHit hit;
        Vector3 direction = waypoints[currentWaypointIndex].position - transform.position;

        // Проверяем, есть ли препятствие между машиной и вейпоинтом
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Waypoint"))
            {
                return true;
            }
        }

        return false;
    }

    bool IsTurning()
    {
        // Проверяем, поворачивает ли машина
        Vector3 direction = (agent.steeringTarget - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        return angle > 30f; // Пусть считается, что поворот начинается при угле больше 30 градусов
    }

    void AdjustNavMeshAgentSize()
    {
        // Настройка размеров NavMeshAgent под размеры BoxCollider
        if (boxCollider != null && agent != null)
        {
            agent.radius = boxCollider.size.x / 2;
            agent.height = boxCollider.size.y;
        }
    }

    void RotateWheels()
    {
        // Вращение передних колес
        if (frontLeftWheel != null && frontRightWheel != null)
        {
            // Определяем угол поворота колес
            float steerAngle = 0f;
            if (rb.velocity.magnitude > 0.1f)
            {
                steerAngle = Vector3.SignedAngle(transform.forward, rb.velocity, Vector3.up);
            }

            // Наклоняем колеса во время поворота
            frontLeftWheel.localRotation = Quaternion.Euler(0, steerAngle, 0);
            frontRightWheel.localRotation = Quaternion.Euler(0, steerAngle, 0);

            // Вращаем колеса
            frontLeftWheel.Rotate(Vector3.right, wheelRotationSpeed * Time.deltaTime);
            frontRightWheel.Rotate(Vector3.right, wheelRotationSpeed * Time.deltaTime);
        }

        // Вращение задних колес
        if (rearLeftWheel != null && rearRightWheel != null)
        {
            rearLeftWheel.Rotate(Vector3.right, wheelRotationSpeed * Time.deltaTime);
            rearRightWheel.Rotate(Vector3.right, wheelRotationSpeed * Time.deltaTime);
        }
    }

    void AddWaypointsFromParent()
    {
        // Получаем все дочерние объекты родительского объекта вейпоинтов
        int childCount = waypointsParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = waypointsParent.GetChild(i);
            waypoints.Add(child);
        }
    }
}
