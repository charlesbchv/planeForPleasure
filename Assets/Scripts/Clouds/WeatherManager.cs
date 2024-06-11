using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public WeatherData clearWeather;
    public WeatherData rainWeather;
    public WeatherData snowWeather;
    public ParticleSystem rainParticles;
    public ParticleSystem snowParticles;
    public CloudMaster cloudMaster;

    private float timeSinceLastChange = 0.0f;
    private float changeIntervalMin = 30.0f; 
    private float changeIntervalMax = 100.0f; 
    void Start()
    {
        ChangeWeather();
    }

    void Update()
    {
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= Random.Range(changeIntervalMin, changeIntervalMax))
        {
            timeSinceLastChange = 0.0f;
            ChangeWeather();
        }
    }

    void ChangeWeather()
    {
        // Stop any currently playing particles
        StopCurrentParticles();

        // Determine new weather based on chance distribution
        int weatherChance = Random.Range(0, 120);
        WeatherData newWeather;
        if (weatherChance < 50) {
            newWeather = clearWeather;
        } else if (weatherChance < 80) {
            newWeather = rainWeather;
        } else {
            newWeather = snowWeather;
        }

        // Apply weather specific settings
        ApplyWeather(newWeather);
    }

    void StopCurrentParticles()
    {
        if (rainParticles != null && rainParticles.isPlaying)
        {
            rainParticles.Stop();
        }
        if (snowParticles != null && snowParticles.isPlaying)
        {
            snowParticles.Stop();
        }
    }

    void ApplyWeather(WeatherData weather)
    {
        Debug.Log(weather.message);
        cloudMaster.densityOffset = weather.cloudDensity;

        if (weather.particleSystem != null)
        {
            var emissionModule = weather.particleSystem.emission;
            emissionModule.rateOverTime = Random.Range(weather.intensityRange[0], weather.intensityRange[1]);
            weather.particleSystem.Play();
        }
    }
}

public class WeatherData
{
    public string message;
    public ParticleSystem particleSystem;
    public float[] intensityRange;
    public float cloudDensity;
}
