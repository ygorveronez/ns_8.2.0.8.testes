using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Colaborador
    {
        [JsonProperty(PropertyName = "type", Required = Required.Default)]
        public int Tipo { get; set; }

        [JsonProperty(PropertyName = "registerNumber", Required = Required.Default)]
        public int NumeroRegistro { get; set; }
    }
}
