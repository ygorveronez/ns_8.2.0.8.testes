using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus
{
    public class RecebimentoCargaStatus
    {
        /// <summary>
        /// Lista de status para atualizar
        /// </summary>
        [JsonProperty("Status")]
        public List<Status> Status { get; set; }
    }
}
