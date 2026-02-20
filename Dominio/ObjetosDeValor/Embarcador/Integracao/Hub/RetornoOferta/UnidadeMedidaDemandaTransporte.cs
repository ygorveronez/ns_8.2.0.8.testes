using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class UnidadeMedidaDemandaTransporte
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("measurementUnitId")]
        public string IdUnidadeMedida { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantidade { get; set; }
    }
}
