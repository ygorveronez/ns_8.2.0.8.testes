using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoNFSe
{
    public class EnvioConfirmacaoNFSe
    {
        [JsonProperty("envia_conf_nfse")]
        public List<NFSeConfirmacao> EnviaConfirmacaoNFSeLista { get; set; }
    }
}
