using Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class RetornoPessoa
    {
        [JsonProperty(PropertyName = "success", Required = Required.Always)]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Always)]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public Result Result { get; set; }
    }

    public class Result
    {
        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "params", Required = Required.Default)]
        public Params Params { get; set; }

        [JsonProperty(PropertyName = "cod_pessoa", Required = Required.Default)]
        public decimal? CodPessoa { get; set; }

        [JsonProperty(PropertyName = "cod_endereco", Required = Required.Default)]
        public decimal? CodEndereco { get; set; }
    }

    public class Params
    {
        [JsonProperty(PropertyName = "cod_pessoa", Required = Required.Default)]
        public int CodPessoa { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "enderecos", Required = Required.Default)]
        public List<EnderecoKMMRetorno> Enderecos { get; set; }

    }
    public class EnderecoKMMRetorno
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public string IdExterno { get; set; }

        [JsonProperty(PropertyName = "cod_endereco", Required = Required.Default)]
        public int? CodEndereco { get; set; }

    }
}
