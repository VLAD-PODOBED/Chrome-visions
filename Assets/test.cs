using UnityEngine;
using UnityEngine.Rendering;

public class LightingManager : MonoBehaviour
{
    void Start()
    {
        // Небольшая задержка перед обновлением
        Invoke("UpdateLightingAndReflections", 0.1f);
    }

    void UpdateLightingAndReflections()
    {
        // Обновляем глобальное освещение
        DynamicGI.UpdateEnvironment();

        // Обновляем все reflection probes в сцене
        ReflectionProbe[] probes = FindObjectsOfType<ReflectionProbe>();
        foreach (ReflectionProbe probe in probes)
        {
            probe.RenderProbe();
        }

        // Обновляем интенсивность освещения окружения
        RenderSettings.ambientIntensity = 1f; // или другое желаемое значение

        // Обновляем интенсивность отражений
        RenderSettings.reflectionIntensity = 1f; // или другое желаемое значение
    }
}