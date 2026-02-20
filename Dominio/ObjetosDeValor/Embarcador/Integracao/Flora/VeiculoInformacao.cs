using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class VeiculoInformacao
    {
        [JsonProperty("Item")]
        public int Item { get; set; }

        [JsonProperty("CdInformacao")]
        public int CodigoEvento { get; set; }

        [JsonProperty("Conteudo")]
        public string Conteudo { get; set; }
    }
}
