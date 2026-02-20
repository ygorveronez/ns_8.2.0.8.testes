using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoBuscaCartao
    {
        [JsonProperty(PropertyName = "content", Required = Required.AllowNull)]
        public List<RetornoBuscaCartaoContent> Content { get; set; }
    }
}
