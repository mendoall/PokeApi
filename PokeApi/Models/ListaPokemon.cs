namespace PokeApi.Models
{
    public class ListaPokemon
    {
        [JsonProperty("results")]
        public List<Pokemon> Resultados { get; set; }
    }
}
