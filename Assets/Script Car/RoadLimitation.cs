using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IgrokController : MonoBehaviour
{
    private bool onRoad = true;
    // Вместо одного флага используем счётчик зон
    private int safeZoneCounter = 0;

    private Rigidbody rb;
    private List<Vector3> positionHistory;
    private List<float> timeHistory;
    private const float timeDelay = 3.0f;
    private const int historyLength = 600;
    // Минимальное расстояние между текущей позицией и точкой телепортации
    private const float safeDistanceThreshold = 50.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        positionHistory = new List<Vector3>(historyLength);
        timeHistory = new List<float>(historyLength);
    }

    private void FixedUpdate()
    {
        onRoad = IsOnRoad();

        // Если игрок не на дороге и не в безопасной зоне, запускаем телепортацию
        if (!onRoad && safeZoneCounter == 0)
        {
            StartCoroutine(TeleportToRoadAfterDelay());
        }
        else
        {
            UpdatePositionHistory();
        }
    }

    private bool IsOnRoad()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.collider.CompareTag("Road"))
            {
                return true;
            }
        }
        return false;
    }

    private void UpdatePositionHistory()
    {
        if (positionHistory.Count >= historyLength)
        {
            positionHistory.RemoveAt(0);
            timeHistory.RemoveAt(0);
        }
        positionHistory.Add(transform.position);
        timeHistory.Add(Time.time);
    }

    public IEnumerator TeleportToRoadAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);

        // Если игрок до сих пор не на дороге и вне безопасной зоны, телепортируем его
        if (!IsOnRoad() && safeZoneCounter == 0)
        {
            Vector3 pastPosition = GetPositionFiveSecondsAgo();
            if (pastPosition != Vector3.zero)
            {
                Debug.Log("Телепортируем игрока (off-road) на позицию: " + pastPosition);
                Vector3 savedVelocity = rb.velocity;
                Vector3 savedAngularVelocity = rb.angularVelocity;
                transform.position = pastPosition;
                rb.velocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;
                rb.Sleep();
                rb.WakeUp();
            }
            else
            {
                Debug.LogWarning("Не удалось получить позицию из истории. Возможно, игрок ещё не проехал достаточное время.");
            }
        }
    }

    public IEnumerator TeleportToRoadAfterDelayOnRoad()
    {
        yield return new WaitForSeconds(0.01f);

        Vector3 pastPosition = GetPositionFiveSecondsAgo();
        if (pastPosition != Vector3.zero)
        {
            Debug.Log("Телепортируем игрока (при столкновении с препятствием) на позицию: " + pastPosition);
            Vector3 savedVelocity = rb.velocity;
            Vector3 savedAngularVelocity = rb.angularVelocity;
            transform.position = pastPosition;
            rb.velocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;
            rb.Sleep();
            rb.WakeUp();
        }
        else
        {
            Debug.LogWarning("Не удалось получить позицию из истории при столкновении с препятствием.");
        }
    }

    // Поиск позиции из истории, которая была не позже Time.time - timeDelay 
    // и отличается от текущей позиции на safeDistanceThreshold или больше.
    public Vector3 GetPositionFiveSecondsAgo()
    {
        float targetTime = Time.time - timeDelay;

        if (timeHistory.Count == 0 || timeHistory[0] > targetTime)
        {
            return Vector3.zero;
        }

        // Ищем с конца (начиная с самой свежей подходящей записи)
        for (int i = timeHistory.Count - 1; i > 0; i--)
        {
            if (timeHistory[i] <= targetTime)
            {
                Vector3 candidate;
                if (i + 1 < timeHistory.Count)
                {
                    float t = (targetTime - timeHistory[i]) / (timeHistory[i + 1] - timeHistory[i]);
                    candidate = Vector3.Lerp(positionHistory[i], positionHistory[i + 1], t);
                }
                else
                {
                    candidate = positionHistory[i];
                }

                if (Vector3.Distance(candidate, transform.position) >= safeDistanceThreshold)
                {
                    return candidate;
                }
                // Если кандидат слишком близко, продолжаем поиск более старых записей
            }
        }

        // Если не найдено ни одной подходящей позиции, можно вернуть самую старую, если она удовлетворяет условию,
        // или Vector3.zero, если и она слишком близко.
        if (positionHistory.Count > 0 && Vector3.Distance(positionHistory[0], transform.position) >= safeDistanceThreshold)
        {
            return positionHistory[0];
        }

        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Увеличиваем счётчик, если зашли в безопасную зону
        if (other.CompareTag("OffRoadSafeZone"))
        {
            safeZoneCounter++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Уменьшаем счётчик, если вышли из безопасной зоны
        if (other.CompareTag("OffRoadSafeZone"))
        {
            safeZoneCounter = Mathf.Max(0, safeZoneCounter - 1);
        }
    }
}
