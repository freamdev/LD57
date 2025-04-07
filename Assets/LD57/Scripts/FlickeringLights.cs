using UnityEngine;

public class FlickeringLights : MonoBehaviour
{
    public Light targetLight;
    public float intensityMin = 1.2f;
    public float intensityMax = 2.0f;
    public float flickerSpeed = 10f;
    public float flickerAmount = 0.5f;

    private float baseIntensity;

    void Start()
    {
        if (!targetLight) targetLight = GetComponent<Light>();
        baseIntensity = targetLight.intensity;
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        targetLight.intensity = Mathf.Lerp(intensityMin, intensityMax, noise) + Random.Range(-flickerAmount, flickerAmount) * 0.1f;
    }
}
