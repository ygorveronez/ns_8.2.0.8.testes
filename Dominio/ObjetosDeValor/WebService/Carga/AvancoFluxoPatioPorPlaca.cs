using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class AvancoFluxoPatioPorPlaca
    {
        [JsonPropertyName("etapaAtualFluxo")]
        public int EtapaAtualFluxo { get; set; }
        
        [JsonPropertyName("placaVeiculo")]
        public string PlacaVeiculo { get; set; }
        
        [JsonPropertyName("dataEtapa")]
        public string DataEtapa { get; set; }
    }
}
