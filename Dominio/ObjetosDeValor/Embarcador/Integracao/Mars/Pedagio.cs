using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class Pedagio
    {
        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("tollValue")]
        public string ValorPedagio { get; set; }

        [JsonProperty("receipt")]
        public string Recibo { get; set; }

    }
}
