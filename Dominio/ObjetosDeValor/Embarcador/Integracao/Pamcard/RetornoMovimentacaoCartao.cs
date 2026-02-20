using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoMovimentacaoCartao
    {
        [JsonProperty(PropertyName = "conteudo", Required = Required.AllowNull)]
        public RetornoMovimentacaoCartaoConteudo Conteudo { get; set; }

        [JsonProperty(PropertyName = "dataAlteracao", Required = Required.AllowNull)]
        public DateTime? DataAlteracao { get; set; }

        [JsonProperty(PropertyName = "dataMovimentacao", Required = Required.AllowNull)]
        public DateTime DataMovimentacao { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public string Id { get; set; }
    }
}
