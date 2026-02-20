using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech
{
    public class ObjetoRetorno
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("msg")]
        public string Mensagem { get; set; }
    }
}
