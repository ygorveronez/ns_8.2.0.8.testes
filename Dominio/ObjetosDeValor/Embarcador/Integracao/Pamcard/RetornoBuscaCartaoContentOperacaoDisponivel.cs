using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoBuscaCartaoContentOperacaoDisponivel
    {
        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.AllowNull)]
        public string Descricao { get; set; }
    }
}
