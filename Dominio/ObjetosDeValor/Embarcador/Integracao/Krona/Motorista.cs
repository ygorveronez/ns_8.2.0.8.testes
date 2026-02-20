using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class Motorista
    {
        [JsonProperty(PropertyName = "aso", Order = 23)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd", true })]
        public DateTime? Aso { get; set; }

        [JsonProperty(PropertyName = "capacitacao", Order = 25)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd", true })]
        public DateTime? Capacitacao { get; set; }

        [JsonProperty(PropertyName = "cdd", Order = 24)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd", true })]
        public DateTime? Cdd { get; set; }

        [JsonProperty(PropertyName = "cnh_categoria", Order = 10, Required = Required.Always)]
        public string CnhCategoria { get; set; }

        [JsonProperty(PropertyName = "cnh_numero", Order = 9, Required = Required.Always)]
        public string CnhNumero { get; set; }

        [JsonProperty(PropertyName = "cnh_vencimento", Order = 11, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd" })]
        public DateTime CnhVencimento { get; set; }

        [JsonProperty(PropertyName = "cpf", Order = 2, Required = Required.Always)]
        public string Cpf { get; set; }

        [JsonProperty(PropertyName = "data_nascimento", Order = 5, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd" })]
        public DateTime DataNascimento { get; set; }

        [JsonProperty(PropertyName = "end_bairro", Order = 15, Required = Required.Always)]
        public string EnderecoBairro { get; set; }

        [JsonProperty(PropertyName = "end_cep", Order = 18, Required = Required.Always)]
        public string EnderecoCep { get; set; }

        [JsonProperty(PropertyName = "end_cidade", Order = 16, Required = Required.Always)]
        public string EnderecoCidade { get; set; }

        [JsonProperty(PropertyName = "end_complemento", Order = 14)]
        public string EnderecoComplemento { get; set; }

        [JsonProperty(PropertyName = "end_numero", Order = 13, Required = Required.Always)]
        public string EnderecoNumero { get; set; }

        [JsonProperty(PropertyName = "end_rua", Order = 12, Required = Required.Always)]
        public string EnderecoRua { get; set; }

        [JsonProperty(PropertyName = "end_uf", Order = 17, Required = Required.Always)]
        public string EnderecoUf { get; set; }

        [JsonProperty(PropertyName = "escolaridade", Order = 8)]
        public string Escolaridade { get; set; }

        [JsonProperty(PropertyName = "estado_civil", Order = 7)]
        public string EstadoCivil { get; set; }

        [JsonProperty(PropertyName = "mopp", Order = 22)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd", true })]
        public DateTime? Mopp { get; set; }

        [JsonProperty(PropertyName = "nextel", Order = 21)]
        public string Nextel { get; set; }

        [JsonProperty(PropertyName = "nome", Order = 1, Required = Required.Always)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "nome_mae", Order = 6)]
        public string NomeMae { get; set; }

        [JsonProperty(PropertyName = "orgao_emissor", Order = 4)]
        public string OrgaoEmissor { get; set; }

        [JsonProperty(PropertyName = "rg", Order = 3, Required = Required.Always)]
        public string Rg { get; set; }

        [JsonProperty(PropertyName = "tel_celular", Order = 20 , Required = Required.Always)]
        public string TelefoneCelular { get; set; }

        [JsonProperty(PropertyName = "tel_fixo", Order = 19)]
        public string TelefoneFixo { get; set; }

        [JsonProperty(PropertyName = "vinculo", Order = 26, Required = Required.Always)]
        public string Vinculo { get; set; }
    }
}
