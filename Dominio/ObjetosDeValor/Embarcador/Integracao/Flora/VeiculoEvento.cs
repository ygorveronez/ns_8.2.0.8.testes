using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class VeiculoEvento
    {
        [JsonProperty("CdEvento")]
        public int CodigoEvento { get; set; }

        [JsonProperty("Valor")]
        public decimal Valor { get; set; }

        [JsonProperty("BloqueadoAlteracao")]
        public string BloqueadoAlteracao { get; set; }

    }
}
