using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class EnvioCTeRetorno
    {
        [JsonProperty("envia_cte_resp")]
        public List<DocumentoCTeRetorno> DocumentosCTeRetorno { get; set; }
    }
}
