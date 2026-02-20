using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal
{
    public class EnvioOcorrencia
    {
        [JsonProperty(PropertyName = "ID_Parceiro", Required = Required.Default)]
        public string CodigoParceiro { get; set; }

        [JsonProperty(PropertyName = "Parceiro", Required = Required.Default)]
        public string Parceiro { get; set; }

        [JsonProperty(PropertyName = "Transportes", Required = Required.Default)]
        public List<EventoTransporte> Transportes { get; set; }
    }
}
