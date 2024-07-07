using UnityEngine;

public class IgrokCollisionHandler : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Limitation"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 pushBackDirection = (transform.position - collision.transform.position).normalized;
            transform.position += pushBackDirection * 0.1f;
        }
    }
}
