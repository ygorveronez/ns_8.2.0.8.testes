using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin
{
    public class RespostaData
    {
        [JsonProperty(PropertyName = "content", Required = Required.Default)]
        public List<RespostaConteudo> Conteudo { get; set; }
    }
}
