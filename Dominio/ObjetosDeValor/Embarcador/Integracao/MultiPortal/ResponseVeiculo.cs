using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal
{
    public class ResponseVeiculo : Response
    {
        [JsonProperty(PropertyName = "object")]
        public List<Veiculo> Veiculos { get; set; }
    }
}
