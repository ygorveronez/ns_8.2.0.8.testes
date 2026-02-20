using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Loggi
{
    public class CTeAnteriorLoggi
    {
        [JsonProperty("cte_anterior")]
        public List<CTeAnteriorLoggiDados> ListaCteAnterior { get; set; }
    }
}
