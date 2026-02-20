using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JJ
{
    public class ChamadoItem
    {
        [JsonProperty(PropertyName = "sku", Required = Required.Default)]
        public string CodigoProduto { get; set; }

        [JsonProperty(PropertyName = "qtde", Required = Required.Default)]
        public int Quantidade { get; set; }

        [JsonProperty(PropertyName = "criticalDate", Required = Required.Default)]
        public string DataCritica { get; set; }

        [JsonProperty(PropertyName = "batch", Required = Required.Default)]
        public string Lote { get; set; }
    }
}
