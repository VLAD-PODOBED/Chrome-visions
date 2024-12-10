using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obsticle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 175f;
    public IgrokController playerController;
    
    private bool canCollide = true;
    private float collisionCooldown = 2f;

    void Start()
    {
       
    }

    void Update()
    {
        // Вращение препятствия вокруг своей оси
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return; // Exit if still in cooldown

        Debug.Log("Collision detected with: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Igrok"))
        {
            if (playerController != null)
            {
                StartCoroutine(playerController.TeleportToRoadAfterDelayOnRoad());
            }
        }

        StartCoroutine(CollisionCooldown());
    }

    private IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(collisionCooldown);
        canCollide = true;
    }
}