using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class Motorista
    {
        [JsonProperty(PropertyName = "cpf", Order = 1, Required = Required.Default)]
        public string Cpf { get; set; }

        [JsonProperty(PropertyName = "nomeCompleto", Order = 2, Required = Required.Default)]
        public string Nome { get; set; }
    }
}
