using UnityEngine;
using System.Collections.Generic;

public class IgrokController : MonoBehaviour
{
    private bool onRoad = true;
    private Rigidbody rb;
    private List<Vector3> positionHistory;
    private List<float> timeHistory;
    private const float timeDelay = 3.0f;
    private const int historyLength = 600;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        positionHistory = new List<Vector3>(historyLength);
        timeHistory = new List<float>(historyLength);
    }

    private void FixedUpdate()
    {
        onRoad = IsOnRoad();

        if (!onRoad)
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

    public System.Collections.IEnumerator TeleportToRoadAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
      

        if (!IsOnRoad())
        {
            Vector3 positionFiveSecondsAgo = GetPositionFiveSecondsAgo();

            if (positionFiveSecondsAgo != Vector3.zero)
            {
                Vector3 savedVelocity = rb.velocity;
                Vector3 savedAngularVelocity = rb.angularVelocity;
                transform.position = positionFiveSecondsAgo;

                rb.velocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;

                rb.Sleep();
                rb.WakeUp();
            }
        }
    }

    public System.Collections.IEnumerator TeleportToRoadAfterDelayOnRoad()
    {
        yield return new WaitForSeconds(0.2f);
       

        
            Vector3 positionFiveSecondsAgo = GetPositionFiveSecondsAgo();

            if (positionFiveSecondsAgo != Vector3.zero)
            {
                Vector3 savedVelocity = rb.velocity;
                Vector3 savedAngularVelocity = rb.angularVelocity;
                transform.position = positionFiveSecondsAgo;

                rb.velocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;

                rb.Sleep();
                rb.WakeUp();
            }
  
    }

    public Vector3 GetPositionFiveSecondsAgo()
    {
        float targetTime = Time.time - timeDelay;

        if (timeHistory.Count == 0 || timeHistory[0] > targetTime)
        {
            return Vector3.zero;
        }

        for (int i = timeHistory.Count - 1; i > 0; i--)
        {
            if (timeHistory[i] <= targetTime)
            {
                if (i + 1 < timeHistory.Count)
                {
                    float t = (targetTime - timeHistory[i]) / (timeHistory[i + 1] - timeHistory[i]);
                    return Vector3.Lerp(positionHistory[i], positionHistory[i + 1], t);
                }
                else
                {
                    return positionHistory[i];
                }
            }
        }

        return positionHistory[0]; 
    }
}