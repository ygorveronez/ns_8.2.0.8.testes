using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido
{
    public class EnviaConfirmacaoPedido
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("envia_conf_ped")]
        public List<ConfiguracaoPedido> ConfiguracaoPedido { get; set; }     

    }
}
