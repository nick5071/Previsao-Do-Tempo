using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Previsão_Do_Tempo.Models;
using System.ComponentModel.DataAnnotations;

namespace Previsão_Do_Tempo.Models
{
        public class ClimaTempo
        {
        [Required(ErrorMessage = "Por favor, informe o nome da cidade.")]
        public string Cidade { get; set; }
            public double TempMax { get; set; }
            public double TempMin { get; set; }
    }

    public class Clima
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public Clima(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApi:ApiKey"];
            _baseUrl = configuration["WeatherApi:BaseUrl"];
        }

        public async Task<ClimaTempo> PegarClimaAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException("O nome da cidade não pode estar vazio.");
            }

            try
            {
                var url = $"{_baseUrl}{Uri.EscapeDataString(location)}?key={_apiKey}&include=days&elements=tempmax,tempmin,precip&unitGroup=metric";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro {response.StatusCode}: {errorContent}");
                }

                var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                var forecast = new ClimaTempo
                {
                    Cidade = location,
                    TempMax = data.days[0].tempmax,
                    TempMin = data.days[0].tempmin,
                };

                return forecast;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Erro ao acessar a API de clima.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado ao processar a requisição.", ex);
            }
        }
    }
}



