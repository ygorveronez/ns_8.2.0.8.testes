using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class CargaDadosTransporteIntegracao
    {
        [JsonProperty("key")]
        public Key Chave { get; set; }

        [JsonProperty("value")]
        public Value ValoresIntegracao { get; set; }
    }
}
