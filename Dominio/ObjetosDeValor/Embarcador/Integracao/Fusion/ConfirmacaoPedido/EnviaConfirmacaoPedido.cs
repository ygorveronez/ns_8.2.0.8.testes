using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido
{
    public class EnviaConfirmacaoPedido
    {
        [JsonProperty("envia_conf_ped")]
        public List<ConfiguracaoPedido> ConfiguracaoPedido { get; set; }
    }
}
