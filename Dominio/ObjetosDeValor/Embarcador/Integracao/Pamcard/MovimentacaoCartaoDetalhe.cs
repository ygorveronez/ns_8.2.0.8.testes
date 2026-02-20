using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class MovimentacaoCartaoDetalhe
    {
        [JsonProperty(PropertyName = "idCartao", Required = Required.Always)]
        public string IdCartao { get; set; }

        [JsonProperty(PropertyName = "idContaCartao", Required = Required.Always)]
        public string IdContaCartao { get; set; }

        [JsonProperty(PropertyName = "idImpresso", Required = Required.Always)]
        public string IdImpresso { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Always)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "proprietario", Required = Required.Always)]
        public MovimentacaoCartaoDetalheProprietario Proprietario { get; set; }

        [JsonProperty(PropertyName = "tipoCartao", Required = Required.Always)]
        public MovimentacaoCartaoDetalheTipoCartao TipoCartao { get; set; }
    }
}
