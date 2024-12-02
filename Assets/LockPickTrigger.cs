using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickTrigger : MonoBehaviour
{
    public GameObject lockPickObject;
    public GameObject lockObject;
    public CanvasGroup fadePanel;
    private ArcadeCarController carController;

    void Start()
    {
        lockPickObject.SetActive(false);
        lockObject.SetActive(false);
        
       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Igrok"))
        {
            carController = other.GetComponent<ArcadeCarController>();
            if (carController != null)
            {
                StartLockpicking();
            }
        }
    }

    private void StartLockpicking()
    {
        lockPickObject.SetActive(true);
        lockObject.SetActive(true);
        
        // StartCoroutine(FadeScreen());
        
        carController.StopCar();
        carController.enabled = false;
    }

    private IEnumerator FadeScreen()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = elapsed / duration;
            yield return null;
        }
        
        fadePanel.alpha = 1f;
    }
}
