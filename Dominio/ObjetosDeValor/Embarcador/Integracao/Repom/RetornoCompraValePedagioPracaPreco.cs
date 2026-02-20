using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCompraValePedagioPracaPreco
    {
        [JsonProperty(PropertyName = "preco", Required = Required.AllowNull)]
        public decimal Preco { get; set; }

        [JsonProperty(PropertyName = "quantidadeEixos", Required = Required.AllowNull)]
        public int QuantidadeEixos { get; set; }
    }
}
