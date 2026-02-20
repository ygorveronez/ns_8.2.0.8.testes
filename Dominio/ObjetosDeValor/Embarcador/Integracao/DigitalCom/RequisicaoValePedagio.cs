using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class RequisicaoValePedagio
    {
        [JsonProperty("routes")]
        public List<Rotas> Rotas { get; set; }
    }
}
