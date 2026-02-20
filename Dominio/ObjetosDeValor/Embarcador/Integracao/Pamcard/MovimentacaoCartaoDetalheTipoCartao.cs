using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class MovimentacaoCartaoDetalheTipoCartao
    {
        [JsonProperty(PropertyName = "emissor", Required = Required.Always)]
        public string Emissor { get; set; }

        [JsonProperty(PropertyName = "modalidade", Required = Required.Always)]
        public string Modalidade { get; set; }
    }
}
