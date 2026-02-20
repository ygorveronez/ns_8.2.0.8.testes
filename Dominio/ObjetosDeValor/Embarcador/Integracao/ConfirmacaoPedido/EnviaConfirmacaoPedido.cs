using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido
{ 
    public class EnviaConfirmacaoPedido
    {
        [JsonProperty("protocoloRequisicao")]
        public string ProtocoloRequisicao { get; set; }

        [JsonProperty("envia_conf_ped")]
        public List<ConfiguracaoPedido> ConfiguracaoPedido { get; set; }
    }
}
