using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class EnvioOutrosDocumentos
    {
        [JsonProperty("envia_cfd")]
        public List<OutrosDocumentos> OutrosDocumentos { get; set; }
    }
}