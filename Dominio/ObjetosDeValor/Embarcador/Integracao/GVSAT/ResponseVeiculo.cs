using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    public class ResponseVeiculo : Response
    {
        [JsonProperty(PropertyName = "object")]
        public List<Veiculo> Veiculos { get; set; }
    }
}
