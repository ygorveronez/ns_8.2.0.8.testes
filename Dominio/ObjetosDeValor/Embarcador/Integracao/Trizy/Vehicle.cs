using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Vehicle
    {
        public string transportMode { get; set; }
        public int axleCount { get; set; }
        public int? plateCount { get; set; }
        public string? label { get; set; }
        public string? licensePlate { get; set; }
        public string? body { get; set; }
        public string? configuration { get; set; }
        public string? type { get; set; }
    }
}