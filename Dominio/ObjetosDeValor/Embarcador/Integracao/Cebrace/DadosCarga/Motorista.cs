using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga
{
    public class Motorista
    {
        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "documento", Required = Required.Default)]
        public string Documento { get; set; }
    }
}