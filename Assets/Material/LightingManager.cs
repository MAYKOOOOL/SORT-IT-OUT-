using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;

    private float weatherDarknessFactor = 1f;
    private float currentTimePercent;

    public void UpdateLighting(float timePercent)
    {
        if (Preset == null) return;

        currentTimePercent = timePercent;

        Color ambient = Preset.AmbientColor.Evaluate(timePercent) * weatherDarknessFactor;
        Color fog = Preset.FogColor.Evaluate(timePercent) * weatherDarknessFactor;
        Color directional = Preset.DirectionalColor.Evaluate(timePercent) * weatherDarknessFactor;

        RenderSettings.ambientLight = ambient;
        RenderSettings.fogColor = fog;

        if (DirectionalLight != null)
        {
            DirectionalLight.color = directional;
            DirectionalLight.transform.localRotation = Quaternion.Euler((timePercent * 360f) - 90f, 170f, 0);
        }
    }

    public void SetWeatherDarkness(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Cloudy:
                weatherDarknessFactor = 0.6f;
                break;
            case WeatherType.Rainy:
                weatherDarknessFactor = 0.7f;
                break;
            case WeatherType.Clear:
            default:
                weatherDarknessFactor = 1f;
                break;
        }

        UpdateLighting(currentTimePercent);
    }
}
