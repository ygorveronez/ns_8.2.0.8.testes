using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class MovimentacaoCartao
    {
        [JsonProperty(PropertyName = "descricao", Required = Required.AllowNull)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "valor", Required = Required.Always)]
        public decimal Valor { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Always)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "tipoEfetivacao", Required = Required.AllowNull)]
        public string TipoEfetivacao { get; set; }

        [JsonProperty(PropertyName = "dataAgendamento", Required = Required.AllowNull)]
        public string DataAgendamento { get; set; }

        [JsonProperty(PropertyName = "cartao", Required = Required.Always)]
        public MovimentacaoCartaoDetalhe Cartao { get; set; }
    }
}
