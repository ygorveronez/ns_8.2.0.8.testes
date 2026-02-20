using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp
{
    public class Componente
    {
        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("parameters")]
        public List<ParametrosComponente> Parametros { get; set; }

    }
}
