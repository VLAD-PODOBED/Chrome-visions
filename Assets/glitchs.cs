using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class glitchs : MonoBehaviour
{
    [SerializeField] private float glitchIntensity = 0.1f; // Сила глитча
    [SerializeField] private float glitchInterval = 0.1f; // Интервал между глитчами
    public float glitchFrequency = 0.3f; // Частота появления глитча
    
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isGlitching = false;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        StartCoroutine(GlitchEffect());
    }

    IEnumerator GlitchEffect()
    {
        while (true)
        {
            // Случайно определяем, будет ли глитч
            if (Random.value < glitchFrequency) // Используем регулируемую частоту
            {
                // Применяем случайное смещение
                transform.position = originalPosition + Random.insideUnitSphere * glitchIntensity;
                transform.rotation = originalRotation * Quaternion.Euler(
                    Random.Range(-10f, 10f) * glitchIntensity,
                    Random.Range(-10f, 10f) * glitchIntensity,
                    Random.Range(-10f, 10f) * glitchIntensity
                );
                
                yield return new WaitForSeconds(glitchInterval);
                
                // Возвращаем в исходное положение
                transform.position = originalPosition;
                transform.rotation = originalRotation;
            }
            
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
