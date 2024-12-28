using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickTrigger : MonoBehaviour
{
    public GameObject lockPickObject;
    public GameObject lockObject;
    public CanvasGroup fadePanel;
    public Camera mainCamera;
    public Camera lockpickCamera;
    private ArcadeCarController carController;

    void Start()
    {
        lockPickObject.SetActive(false);
        lockObject.SetActive(false);
        lockpickCamera.gameObject.SetActive(false);
        fadePanel.alpha = 0f;
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
        StartCoroutine(SwitchToCameraSequence());
    }

    public void ReturnToPlayer()
    {
        StartCoroutine(ReturnToCameraSequence());
    }

    private IEnumerator SwitchToCameraSequence()
    {
        yield return StartCoroutine(FadeScreen(0f, 1f));
        
        lockPickObject.SetActive(true);
        lockObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        lockpickCamera.gameObject.SetActive(true);
        
        carController.StopCar();
        carController.enabled = false;
        
        yield return StartCoroutine(FadeScreen(1f, 0f));
    }

    private IEnumerator ReturnToCameraSequence()
    {
        yield return StartCoroutine(FadeScreen(0f, 1f));
        
        lockPickObject.SetActive(false);
        lockObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        lockpickCamera.gameObject.SetActive(false);
        
        carController.enabled = true;
        
        yield return StartCoroutine(FadeScreen(1f, 0f));
    }

    private IEnumerator FadeScreen(float startAlpha, float targetAlpha)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        
        fadePanel.alpha = targetAlpha;
    }
}
