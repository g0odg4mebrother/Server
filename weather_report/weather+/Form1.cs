using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace weather_
{
    public partial class Form1 : Form
    {
        private readonly string apiKey = "188ce3f4b751a24a7a3c7a0c92db7bc4";

        public Form1()
        {
            InitializeComponent();
            
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            string city = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("Пожалуйста, введите название города.");
                return;
            }

            try
            {
                var coordinates = await GetCoordinatesAsync(city);
                if (coordinates == null)
                {
                    MessageBox.Show("Город не найден.");
                    return;
                }

                double lat = coordinates.Item1;
                double lon = coordinates.Item2;

                listBox1.Items.Clear(); 
                await ShowCurrentWeatherAsync(city, lat, lon);
                await ShowForecastAsync(lat, lon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async Task<Tuple<double, double>> GetCoordinatesAsync(string city)
        {
            string url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={apiKey}";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<dynamic>(json);
                if (data != null && data.Count > 0)
                {
                    double lat = data[0].lat;
                    double lon = data[0].lon;
                    return Tuple.Create(lat, lon);
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task ShowCurrentWeatherAsync(string city, double lat, double lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={apiKey}";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<dynamic>(json);

                string weatherDescription = data.weather[0].description;
                double temp = data.main.temp;

                listBox1.Items.Add($"Сегодня, {city}: {temp:F1} °C, {weatherDescription}");
            }
        }

        private async Task ShowForecastAsync(double lat, double lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude=current,minutely,hourly,alerts&units=metric&appid={apiKey}";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);
                dynamic data = JsonConvert.DeserializeObject(json);

                foreach (var day in data.daily)
                {
                    DateTime date = DateTimeOffset.FromUnixTimeSeconds((long)day.dt).DateTime;
                    string dateStr = date.ToString("dd.MM");
                    string description = day.weather[0].description;
                    string tempDay = day.temp.day.ToString("F1");

                    listBox1.Items.Add($"{dateStr}: {tempDay} °C, {description}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }
    }

}
