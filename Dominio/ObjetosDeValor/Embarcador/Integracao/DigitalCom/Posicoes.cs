using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class Posicoes
    {
        [JsonProperty("position")]
        public List<Posicao> Posicao { get; set; }
    }
}
