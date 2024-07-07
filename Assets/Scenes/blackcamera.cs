using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFadeIn : MonoBehaviour
{
    public float fadeDuration = 2f; // Длительность эффекта затемнения
    public Color fadeColor = Color.black; // Цвет затемнения (обычно черный)

    private Texture2D fadeTexture;
    private float fadeAmount = 1f;

    private void Start()
    {
        StartCoroutine(FadeIn());
        CreateFadeTexture();
    }

    private void CreateFadeTexture()
    {
        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, fadeColor);
        fadeTexture.Apply();
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeAmount = 1f - (elapsedTime / fadeDuration);
            yield return null;
        }

        fadeAmount = 0f;
    }

    private void OnGUI()
    {
        if (fadeAmount > 0f)
        {
            Color guiColor = GUI.color;
            GUI.color = new Color(guiColor.r, guiColor.g, guiColor.b, fadeAmount);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
            GUI.color = guiColor;
        }
    }
}