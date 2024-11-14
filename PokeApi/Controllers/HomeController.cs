using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PokeApi.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace PokeApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string respuesta = ApiRequest("https://pokeapi.co/api/v2/pokemon?limit=151");
            ListaPokemon resultados = JsonConvert.DeserializeObject<ListaPokemon>(respuesta);

            if (resultados?.Resultados == null)
            {
                _logger.LogError("No se encontraron resultados en la respuesta de la API.");
                return View("Error");
            }
            
            List<PokemonDetalle> listaDetalles = new List<PokemonDetalle>();

            foreach (var item in resultados.Resultados)
            {
                string respuestaDetalle = ApiRequest(item.Url);
                PokemonDetalle detalle = JsonConvert.DeserializeObject<PokemonDetalle>(respuestaDetalle);
                listaDetalles.Add(detalle);
            }
            ViewBag.ListaDetalles = listaDetalles;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string ApiRequest(string url)
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer; 
        }
    }
}
