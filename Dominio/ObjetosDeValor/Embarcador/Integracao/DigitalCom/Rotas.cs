using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class Rotas
    {
        [JsonProperty("route")]
        public List<Rota> Rota { get; set; }
    }
}
