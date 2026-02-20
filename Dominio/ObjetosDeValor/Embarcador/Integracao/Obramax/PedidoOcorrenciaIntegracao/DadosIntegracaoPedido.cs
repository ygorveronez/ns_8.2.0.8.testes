using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class DadosIntegracaoPedido
    {
        [JsonProperty("CodVento")]
        public int CodigoEvento { get; set; }

        [JsonProperty("DataEvento")]
        public string DataEvento { get; set; }

        [JsonProperty("Descricao")]
        public string DescricaoOcorrencia { get; set; }

        [JsonProperty("Evento")]
        public string Evento { get; set; }

        [JsonProperty("hora")]
        public string Hora { get; set; }

        [JsonProperty("ImagemCanhoto")]
        public string ImagemCanhoto { get; set; }
        
        [JsonProperty("ImagemEvento")]
        public string ImagemEvento { get; set; }

        [JsonProperty("Protocolo")]
        public string Protocolo { get; set; }

        [JsonProperty("Remessa")]
        public RemessaPedido Remessa { get; set; }
    }
}
