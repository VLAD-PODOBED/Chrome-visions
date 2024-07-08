using System.Collections;
using UnityEngine;

public class BonusTwoRespawn : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private RespawnManager respawnManager;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        respawnManager = GameObject.FindObjectOfType<RespawnManager>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void StartRespawn()
    {
        respawnManager.RespawnObject(gameObject, originalPosition, originalRotation, originalScale, 3f);
    }


   void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.CompareTag("Igrok"))
        {
            PlayPickupSound();
            StartRespawn();
            gameObject.SetActive(false);
        }
    }

    
    void PlayPickupSound()
    {
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }
}