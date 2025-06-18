using System;
using UnityEngine;
using Random = System.Random;
namespace VARLab.CCSIF
{
    //This class controls the weather of the sim. 
    public class WeatherController : MonoBehaviour
    {
        public bool IsSeeded;
        public WeatherType CurrentWeather;
        [SerializeField] private GameObject rainSystemObject;
        [SerializeField] private Material SunnySkyBox;
        [SerializeField] private Material RainySkyBox; 
        [SerializeField] private Light SceneLight;

        private const float SunnyLightIntensity = 2; //more powerful intensity for clear sky
        private const float RainyLightIntensity = 0.9f; //less powerful intensity for cloudy sky
        private const int SunnyLightTemperature = 5000; //slightly yellow colour for clear sky
        private const int RainyLightTemperature = 6500; //slightly blue colour for cloudy sky

        private void Start()
        {
            if (!IsSeeded)
            {
                //Gets a random current weather from any type of weather in the enum
                Array values = Enum.GetValues(typeof(WeatherType));
                Random random = new Random();
                WeatherType randomWeather = (WeatherType)values.GetValue(random.Next(values.Length));
                CurrentWeather = randomWeather;
            }
            WeatherHandler(CurrentWeather);
        }

        //Given a weather type to become, switch to that weather
        //Implemented as a switch case to easily add future weather conditions as needed
        public void WeatherHandler(WeatherType setWeather)
        {
            switch (setWeather)
            {
                case WeatherType.Sunny: ChangeWeather(false, SunnySkyBox, SunnyLightIntensity, SunnyLightTemperature); 
                    break;
                case WeatherType.Rainy: ChangeWeather(true, RainySkyBox, RainyLightIntensity, RainyLightTemperature); 
                    break;
                default:
                    Debug.Log("Error: Weather type not found. public void WeatherHandler(WeatherType setWeather)");
                    break;
            }
        }

        //Helper function for WeatherHandler
        private void ChangeWeather(bool isRaining, Material skyBox, float intensity, int temperature)
        {
            rainSystemObject.SetActive(isRaining);
            RenderSettings.skybox = skyBox;
            SceneLight.intensity = intensity;
            SceneLight.colorTemperature = temperature;
        }
    }
}
