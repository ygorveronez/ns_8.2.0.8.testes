using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class RetornoPedidosIntegracaoCarga
    {
        [JsonProperty("chavelayout")]
        public string chavelayout { get; set; }

        [JsonProperty("list")]
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporte> ListaIntegrarCargaDadosTransporte { get; set; }
    }
}
