using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoConsultaValorPedagio
    {
        [JsonProperty(PropertyName = "valorTotalVpr")]
        public decimal ValorTotalVpr { get; set; }

        [JsonProperty(PropertyName = "valorVprIda")]
        public decimal ValorVprIda { get; set; }

        [JsonProperty(PropertyName = "valorVprVolta")]
        public decimal ValorVprVolta { get; set; }

        [JsonProperty(PropertyName = "descricaoPercurso")]
        public string DescricaoPercurso { get; set; }
    }
}
