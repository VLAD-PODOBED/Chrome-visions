using System.Collections;
using UnityEngine;

public class HallucinationTrigger : MonoBehaviour
{
    [Header("Настройки эффектов")]
    [SerializeField] private float effectDuration = 2f;
    [SerializeField] private float transitionSpeed = 1f;
    
    [Header("Posterize эффект")]
    [SerializeField] private int maxPosterizeLevel = 4;
    [SerializeField] private int defaultPosterizeLevel = 256;
    
    [Header("RGB Shift эффект")]
    [SerializeField] private float maxRGBShiftAmount = 0.1f;
    [SerializeField] private float defaultRGBShiftAmount = 0f;
    
    [Header("Звуки")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hallucinationSound;
    [SerializeField] private float maxVolume = 1f;
    
    private PosterizeEffect posterizeEffect;
    private RGBShiftEffect rgbShiftEffect;
    
    private bool isActive = false;
    private Coroutine effectCoroutine;

    private void Start()
    {
        // Находим эффекты на камере
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            posterizeEffect = mainCamera.GetComponent<PosterizeEffect>();
            rgbShiftEffect = mainCamera.GetComponent<RGBShiftEffect>();
            
            if (posterizeEffect == null)
            {
                Debug.LogError("PosterizeEffect не найден на главной камере!");
            }
            
            if (rgbShiftEffect == null)
            {
                Debug.LogError("RGBShiftEffect не найден на главной камере!");
            }
            
            // Сбрасываем эффекты в начале
            ResetEffects();
        }
        else
        {
            Debug.LogError("Главная камера не найдена!");
        }
        Debug.Log("все ок");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, является ли вошедший объект игроком
        if (other.CompareTag("Igrok") && !isActive)
        {
            if (effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
                 Debug.Log("началось");
            }
            
            effectCoroutine = StartCoroutine(ApplyHallucinationEffect());
        }
    }

    private IEnumerator ApplyHallucinationEffect()
    {
        isActive = true;
        
        // Проигрываем звук
        if (audioSource != null && hallucinationSound != null)
        {
            audioSource.clip = hallucinationSound;
            audioSource.volume = 0;
            audioSource.Play();
        }
        
        // Запускаем эффект постепенного появления
        float timer = 0;
        while (timer < transitionSpeed)
        {
            float t = timer / transitionSpeed;
            
            // Применяем Posterize эффект
            if (posterizeEffect != null)
            {
                // Интерполируем уровень постеризации от высокого (256 - почти без эффекта) к низкому (сильный эффект)
                posterizeEffect.level = Mathf.RoundToInt(Mathf.Lerp(defaultPosterizeLevel, maxPosterizeLevel, t));
            }
            
            // Применяем RGB Shift эффект
            if (rgbShiftEffect != null)
            {
                rgbShiftEffect.amount = Mathf.Lerp(defaultRGBShiftAmount, maxRGBShiftAmount, t);
            }
            
            // Увеличиваем громкость звука
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.volume = Mathf.Lerp(0, maxVolume, t);
            }
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Ждем указанное время для полного эффекта
        yield return new WaitForSeconds(effectDuration);
        
        // Плавное исчезновение эффекта
        timer = 0;
        while (timer < transitionSpeed)
        {
            float t = timer / transitionSpeed;
            
            // Постепенно возвращаем Posterize эффект к норме
            if (posterizeEffect != null)
            {
                posterizeEffect.level = Mathf.RoundToInt(Mathf.Lerp(maxPosterizeLevel, defaultPosterizeLevel, t));
            }
            
            // Постепенно возвращаем RGB Shift эффект к норме
            if (rgbShiftEffect != null)
            {
                rgbShiftEffect.amount = Mathf.Lerp(maxRGBShiftAmount, defaultRGBShiftAmount, t);
            }
            
            // Снижаем громкость звука
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.volume = Mathf.Lerp(maxVolume, 0, t);
            }
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Сбрасываем все эффекты полностью
        ResetEffects();
        
        // Останавливаем звук
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.volume = 0;
        }
        
        isActive = false;
    }
    
    private void ResetEffects()
    {
        // Сбрасываем Posterize эффект
        if (posterizeEffect != null)
        {
            posterizeEffect.level = defaultPosterizeLevel;
        }
        
        // Сбрасываем RGB Shift эффект
        if (rgbShiftEffect != null)
        {
            rgbShiftEffect.amount = defaultRGBShiftAmount;
        }
    }
}