using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga
{
    public class EnviaConfirmacaoFecharCarga
    {
        [JsonProperty("envia_conf_fecha_carga")]
        public List<ConfirmacaoFecharCarga> ConfirmacaoFecharCarga { get; set; }
    }
}
