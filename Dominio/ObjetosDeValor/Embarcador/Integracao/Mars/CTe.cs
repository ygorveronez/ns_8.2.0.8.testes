using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class CTe
    {
        [JsonProperty("CteNumber")]
        public string Numero { get; set; }

        [JsonProperty("ReleaseDate")]
        public DateTime? DataUltimaDigitalizacaoCanhotos { get; set; }
    }
}
