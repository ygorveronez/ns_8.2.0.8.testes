using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class EnvioOutrosDocumentosRetorno
    {
        [JsonProperty("envia_cfd_resp")]
        public List<OutrosDocumentosRetorno> Retorno { get; set; }
    }
}