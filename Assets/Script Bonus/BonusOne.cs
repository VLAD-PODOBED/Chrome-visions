using UnityEngine;
using System.Collections;

public class BonusOneRespawn : MonoBehaviour
{
    private Renderer[] renderers;
    private Collider[] colliders;
    private bool isActive = true;
    
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip pickupSound; 

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        

        if (isActive && (collision.gameObject.CompareTag("Igrok")))
        {
            PlayPickupSound();
            StartCoroutine(RespawnAfterDelay());
            isActive = false;
        }
    }

    void PlayPickupSound()
    {
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        SetObjectActive(false);

        yield return new WaitForSeconds(3f);

        SetObjectActive(true);

        isActive = true;
    }

    void SetObjectActive(bool active)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = active;
        }

        foreach (var collider in colliders)
        {
            collider.enabled = active;
        }
    }
}