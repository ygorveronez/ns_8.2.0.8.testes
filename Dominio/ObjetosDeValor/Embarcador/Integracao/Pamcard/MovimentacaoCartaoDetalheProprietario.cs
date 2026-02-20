using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class MovimentacaoCartaoDetalheProprietario
    {
        [JsonProperty(PropertyName = "documento", Required = Required.Always)]
        public string Documento { get; set; }
    }
}
