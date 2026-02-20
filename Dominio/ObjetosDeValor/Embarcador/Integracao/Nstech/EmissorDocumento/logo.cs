using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class logo
    {
        public string issuerDocument { get; set; }

        [JsonProperty("logo")]
        public string logobase64 { get; set; }
    }
}