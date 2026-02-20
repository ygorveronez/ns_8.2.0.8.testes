using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class VeiculoTerrestre
    {
        [JsonProperty("plate")]
        public string Placa { get; set; }
    }
}
