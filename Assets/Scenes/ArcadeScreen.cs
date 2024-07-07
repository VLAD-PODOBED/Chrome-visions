using UnityEngine;
using UnityEngine.UI;

public class RetroArcadeScreen : MonoBehaviour
{
    public RawImage backgroundImage;
    public float scanlineSpeed = 0.1f;
    public float flickerIntensity = 0.05f;
    public float curvatureIntensity = 0.1f;

    private Material material;
    private float time;

    void Start()
    {
        material = new Material(Shader.Find("Unlit/RetroArcadeScreen"));
        backgroundImage.material = material;
    }

    void Update()
    {
        time += Time.deltaTime;

        material.SetFloat("_ScanlineOffset", time * scanlineSpeed);
        material.SetFloat("_FlickerIntensity", flickerIntensity);
        material.SetFloat("_CurvatureIntensity", curvatureIntensity);
    }
}