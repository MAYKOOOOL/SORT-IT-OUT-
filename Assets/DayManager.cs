using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum WeatherType { Clear, Cloudy, Rainy }

public class DayTimeSystem : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField, Range(0, 24)] private float timeOfDay = 5.9f;
    [SerializeField] private float dayDurationInSeconds = 120f;
    private float timeMultiplier;
    private bool isDaytime = true;

    [Header("Day UI")]
    public TextMeshProUGUI dayText;
    public Image timeImage;
    public Sprite[] timeSprites;

    [Header("Weather UI")]
    public Image weatherImage;
    public Sprite[] weatherSprites;

    [Header("Lighting")]
    public LightingManager lightingManager;

    private int dayCount = 0;
    private float weatherTimer = 0f;
    public WeatherType currentWeather = WeatherType.Clear;

    public event Action<int> OnNewDay;
    public event Action<WeatherType> OnWeatherChanged;

    void Start()
    {
        timeMultiplier = 24f / dayDurationInSeconds;
        ApplyWeather(currentWeather); // Start with Clear
        UpdateDayUI();
    }

    void Update()
    {
        UpdateTime();
        HandleDayTransition();
        UpdateWeatherTimer();
    }

    private void UpdateTime()
    {
        timeOfDay += Time.deltaTime * timeMultiplier;
        timeOfDay %= 24f;
        lightingManager?.UpdateLighting(timeOfDay / 24f);
    }

    private void HandleDayTransition()
    {
        bool isCurrentlyDay = timeOfDay >= 6f && timeOfDay < 18f;

        if (!isDaytime && isCurrentlyDay)
        {
            dayCount++;
            UpdateDayUI();
            OnNewDay?.Invoke(dayCount);
            UpdateTimeSprite(true);
        }
        else if (isDaytime && !isCurrentlyDay)
        {
            UpdateTimeSprite(false);
        }

        isDaytime = isCurrentlyDay;
    }

    private void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"Day: {dayCount}/30";
    }

    private void UpdateTimeSprite(bool isDay)
    {
        if (timeImage != null && timeSprites.Length >= 2)
        {
            timeImage.sprite = isDay ? timeSprites[0] : timeSprites[1];
        }
    }

    private void UpdateWeatherTimer()
    {
        weatherTimer -= Time.deltaTime;

        if (weatherTimer <= 0f)
        {
            WeatherType newWeather;
            do
            {
                newWeather = (WeatherType)UnityEngine.Random.Range(0, 3);
            } while (newWeather == currentWeather);

            ApplyWeather(newWeather);
        }
    }

    public void ApplyWeather(WeatherType newWeather, float customDuration = -1f)
    {
        currentWeather = newWeather;

        weatherTimer = customDuration > 0 ? customDuration : newWeather switch
        {
            WeatherType.Clear => UnityEngine.Random.Range(20f, 40f),
            WeatherType.Cloudy => UnityEngine.Random.Range(15f, 30f),
            WeatherType.Rainy => UnityEngine.Random.Range(10f, 25f),
            _ => 30f
        };

        UpdateWeatherUI();
        lightingManager?.SetWeatherDarkness(newWeather);
        OnWeatherChanged?.Invoke(newWeather);
    }

    private void UpdateWeatherUI()
    {
        int index = currentWeather switch
        {
            WeatherType.Cloudy => 0,
            WeatherType.Rainy => 1,
            _ => 2 // Clear
        };

        if (weatherImage && weatherSprites.Length > index)
        {
            weatherImage.sprite = weatherSprites[index];
            weatherImage.enabled = true;
        }
    }

    public int GetCurrentDay() => dayCount;
    public WeatherType GetCurrentWeather() => currentWeather;
}
