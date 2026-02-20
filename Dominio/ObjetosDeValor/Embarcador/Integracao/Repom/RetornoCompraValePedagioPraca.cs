using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCompraValePedagioPraca
    {
        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "nomeConcessionaria", Required = Required.AllowNull)]
        public string NomeConcessionaria { get; set; }

        [JsonProperty(PropertyName = "concessionariaCodigo", Required = Required.AllowNull)]
        public int ConcessionariaCodigo { get; set; }

        [JsonProperty(PropertyName = "nomePraca", Required = Required.AllowNull)]
        public string NomePraca { get; set; }

        [JsonProperty(PropertyName = "nomeRodovia", Required = Required.AllowNull)]
        public string NomeRodovia { get; set; }

        [JsonProperty(PropertyName = "ativa", Required = Required.AllowNull)]
        public int Ativa { get; set; }

        [JsonProperty(PropertyName = "pracaPreco", Required = Required.AllowNull)]
        public List<RetornoCompraValePedagioPracaPreco> PracaPreco { get; set; }

        [JsonProperty(PropertyName = "result", Required = Required.AllowNull)]
        public RetornoIntegracaoResult Result { get; set; }
    }
}
