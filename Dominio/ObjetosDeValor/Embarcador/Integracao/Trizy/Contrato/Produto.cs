using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ProdutoContrato
    {
        [JsonProperty(PropertyName = "descricao", Required = Required.Default)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "ncm", Required = Required.Default)]
        public string NCM { get; set; }
    }
}
