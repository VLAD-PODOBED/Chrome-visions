using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndTrigger : MonoBehaviour
{
    public CanvasGroup fadePanel;

    void Start()
    {
        fadePanel.alpha = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Igrok"))
        {
            StartCoroutine(QuitGame());
        }
    }

    public IEnumerator QuitGame()
    {
        yield return StartCoroutine(FadeScreen(0f, 1f));

        yield return new WaitForSeconds(1f);
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator FadeScreen(float startAlpha, float targetAlpha)
    {
        float duration = 1.5f;
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