using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoCTe
{
    public class EnvioConfirmacaoCTe
    {
        [JsonProperty("envia_conf_cte")]
        public List<CTeConfirmacao> EnviaConfirmacaoCteLista { get; set; }
    }
}
