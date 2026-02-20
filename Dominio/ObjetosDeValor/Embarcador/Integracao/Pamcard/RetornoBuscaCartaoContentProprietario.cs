using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoBuscaCartaoContentProprietario
    {
        [JsonProperty(PropertyName = "cnpj", Required = Required.AllowNull)]
        public string Cnpj { get; set; }
    }
}
