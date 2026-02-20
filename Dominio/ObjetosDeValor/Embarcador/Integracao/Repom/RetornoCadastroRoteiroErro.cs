using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCadastroRoteiroErro
    {
        [JsonProperty(PropertyName = "isValid", Required = Required.AllowNull)]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "errors", Required = Required.AllowNull)]
        public List<RetornoCadastroRoteiroErroDetalhe> Errors { get; set; }
    }
}
