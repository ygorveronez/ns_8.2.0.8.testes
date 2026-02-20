using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class PedidosAtualizacaoSituacao
    {
        [JsonProperty("orderNumbers")]
        public List<string> OrderNumbers { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }
    }
}
