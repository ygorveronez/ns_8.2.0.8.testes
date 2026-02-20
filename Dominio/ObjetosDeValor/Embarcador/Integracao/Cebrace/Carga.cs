using Newtonsoft.Json;
using System.Collections.Generic;


namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace
{
    public class Carga
    {
        [JsonProperty(PropertyName = "protocolointegracaoCarga")]
        public string ProtocoloIntegracaoCarga { get; set; }

        [JsonProperty(PropertyName = "protocolointegracaoPedido")]
        public List<int> ProtocoloIntegracaoPedido { get; set; }

        public decimal FreteTotal { get; set; }

        public string IVA { get; set; }

        public string TipoOperacao { get; set; }

        public string CodigoIntegracao { get; set; }

        public decimal PedagioValor { get; set; }

        public string PedagioID { get; set; }
    }
}
