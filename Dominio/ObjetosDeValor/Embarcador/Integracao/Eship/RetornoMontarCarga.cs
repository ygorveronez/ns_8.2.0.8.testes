using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCarga
    {
        [JsonProperty(PropertyName = "erros", Required = Required.Default)]
        public List<RetornoMontarCargaErros> Erros { get; set; }

        [JsonProperty(PropertyName = "mensagens", Required = Required.Default)]
        public List<RetornoMontarCargaMensagens> Mensagens { get; set; }

        [JsonProperty(PropertyName = "corpo", Required = Required.Default)]
        public RetornoMontarCargaCorpo Corpo { get; set; }

    }
}
