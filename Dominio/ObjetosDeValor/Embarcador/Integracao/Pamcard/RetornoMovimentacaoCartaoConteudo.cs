using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoMovimentacaoCartaoConteudo
    {
        [JsonProperty(PropertyName = "bilhete", Required = Required.AllowNull)]
        public RetornoMovimentacaoCartaoConteudoBilhete Bilhete { get; set; }

        /*[JsonProperty(PropertyName = "cartao", Required = Required.AllowNull)]
        public RetornoMovimentacaoCartaoConteudo Cartao { get; set; }*/

        [JsonProperty(PropertyName = "dataAgendamento", Required = Required.AllowNull)]
        public DateTime DataAgendamento { get; set; }

        [JsonProperty(PropertyName = "dataMovimento", Required = Required.AllowNull)]
        public DateTime DataMovimento { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.AllowNull)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "grupoStatus", Required = Required.AllowNull)]
        public string GrupoStatus { get; set; }

        [JsonProperty(PropertyName = "insercao", Required = Required.AllowNull)]
        public bool Insercao { get; set; }

        [JsonProperty(PropertyName = "motivo", Required = Required.AllowNull)]
        public string Motivo { get; set; }

        [JsonProperty(PropertyName = "percentualTarifa", Required = Required.AllowNull)]
        public decimal PercentualTarifa { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.AllowNull)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "tipoEfetivacao", Required = Required.AllowNull)]
        public string TipoEfetivacao { get; set; }

        [JsonProperty(PropertyName = "valor", Required = Required.AllowNull)]
        public decimal Valor { get; set; }
    }
}
