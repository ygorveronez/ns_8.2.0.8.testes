using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class RetornoValePedagio
    {
        [JsonProperty("dadosRetorno")]
        public List<DadosRetorno> DadosRetorno { get; set; }
    }
}
