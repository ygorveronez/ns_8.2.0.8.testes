using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class Produtos
    {
        [JsonProperty("Sku")]
        public string CodigoProduto { get; set; }
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        [JsonProperty("Un")]
        public string Unidade { get; set; }
    }
}
