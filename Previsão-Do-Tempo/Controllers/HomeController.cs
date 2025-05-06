using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Previsão_Do_Tempo.Models;

namespace PrevisaoDoTempo.Controllers
{
    public class HomeController : Controller
    {
        private readonly Clima _climaService;

        public HomeController(Clima climaService)
        {
            _climaService = climaService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PegarClima(string location)
        {
            try
            {
                var forecast = await _climaService.PegarClimaAsync(location);
                ViewBag.Forecast = forecast;
                return View("Index");
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Erro ao acessar a API de clima. Por favor, tente novamente mais tarde.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Insira uma cidade válida.";
                return View("Index");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
