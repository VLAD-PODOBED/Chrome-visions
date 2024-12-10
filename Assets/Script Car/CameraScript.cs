using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform Igrok;
    [SerializeField] private Vector3 offset = new Vector3();
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;

    private void LateUpdate()
    {
        Vector3 targetPosition = Igrok.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        Vector3 lookAtPosition = Igrok.position + Vector3.up * 0.6f;
        Vector3 direction = lookAtPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }
}
