using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis
{
    public class Carga
    {
        [JsonProperty("numeroCarga")]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty("protocoloIntegracaoCarga")]
        public int CodigoIntegracaoCarga { get; set; }

        [JsonProperty("tipoOperacaoDescricao")]
        public string TipoOperacao { get; set; }

        [JsonProperty("remessas")]
        public List<Pedido> Pedidos { get; set; }
    }
}
