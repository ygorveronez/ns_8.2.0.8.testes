using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class LoteProvisao
    {
        public string ProtocoloCarga { get; set; }

        [JsonProperty("remessa")]
        public List<Provisao> Provisoes { get; set; }
    }
}
