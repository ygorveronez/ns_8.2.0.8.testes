using Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class RetornoPadraoV2
    {
        [JsonProperty(PropertyName = "success", Required = Required.Always)]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Always)]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public ResultPadraoV2 Result { get; set; }
    }

    public class ResultPadraoV2
    {
        [JsonProperty(PropertyName = "success", Required = Required.Default)]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "params", Required = Required.Default)]
        public ParamsPadraoV2 Params { get; set; }

        [JsonProperty(PropertyName = "lancto_id", Required = Required.Default)]
        public int? LancamentoID { get; set; }
    }

    public class ParamsPadraoV2
    {
        [JsonProperty(PropertyName = "lancto_id", Required = Required.Default)]
        public int? LancamentoID { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }


    }
}
