using UnityEngine;
using SimpleJSON;
using Sirenix.OdinInspector;
using Toolbox.Singleton;
using Toolbox.Events;
using Toolbox.API;

namespace Toolbox.Weather
{
    public enum WeatherType { Clear, Thunderstorm, Drizzle, Rain, Snow, Clouds, Mist, Smoke, Haze, Dust, Fog, Sand, Ash, Squall, Tornado, Atmosphere }

    public class RealWeather : Singleton<RealWeather>
    {
        [SerializeField] readonly string API_URL = "https://api.openweathermap.org/data/2.5/weather";
        [SerializeField] readonly string API_KEY = "10646316d5ba5eb7046537038e57a21b";

        [SerializeField] double latitude;
        [SerializeField] double longitude;
        [SerializeField] WeatherType weather;
        [SerializeField] float temperature;

        public WeatherType Weather => weather;
        public float Temperature => temperature;

        [Header("Broadcasting on ...")]
        public VoidChannelSO OnWeatherUpdated;

        public static void RequestUpdate() => Instance.SendRequest();

        [Button("Fetch Data")]
        void SendRequest() => APIReader.Read(
            $"{API_URL}?lat={latitude}&lon={longitude}&appid={API_KEY}",
            SetServerWeather);

        void SetServerWeather(JSONNode root)
        {
            if (root == null)
                return;

            string weather = root["weather"][0]["main"];

            if (System.Enum.TryParse(weather, out WeatherType type))
                this.weather = type;
            else
                this.LogWarning($"{this.weather} doesn't exist", debug);

            temperature = float.Parse((root["main"]["temp"]).ToString().Replace('.', ','));
            temperature -= 273.15f;

            OnWeatherUpdated?.Invoke();
        }
    }
}