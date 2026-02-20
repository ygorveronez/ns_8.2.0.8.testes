using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class Entidade
    {
        [JsonProperty(PropertyName = "cnpj", Order = 2, Required = Required.Always)]
        public string Cnpj { get; set; }

        [JsonProperty(PropertyName = "codigo", Order = 6)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "end_bairro", Order = 10, Required = Required.Always)]
        public string EnderecoBairro { get; set; }

        [JsonProperty(PropertyName = "end_cep", Order = 13, Required = Required.Always)]
        public string EnderecoCep { get; set; }

        [JsonProperty(PropertyName = "end_cidade", Order = 11, Required = Required.Always)]
        public string EnderecoCidade { get; set; }

        [JsonProperty(PropertyName = "end_complemento", Order = 9)]
        public string EnderecoComplemento { get; set; }

        [JsonProperty(PropertyName = "end_numero", Order = 8, Required = Required.Always)]
        public string EnderecoNumero { get; set; }

        [JsonProperty(PropertyName = "end_rua", Order = 7, Required = Required.Always)]
        public string EnderecoRua { get; set; }

        [JsonProperty(PropertyName = "end_uf", Order = 12, Required = Required.Always)]
        public string EnderecoUf { get; set; }

        [JsonProperty(PropertyName = "latitude", Order = 14)]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude", Order = 15)]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "nome_fantasia", Order = 4, Required = Required.Always)]
        public string NomeFantasia { get; set; }

        [JsonProperty(PropertyName = "razao_social", Order = 3, Required = Required.Always)]
        public string RazaoSocial { get; set; }

        [JsonProperty(PropertyName = "responsavel", Order = 18)]
        public string Responsavel { get; set; }

        [JsonProperty(PropertyName = "responsavel_cargo", Order = 19)]
        public string ResponsavelCargo { get; set; }

        [JsonProperty(PropertyName = "responsavel_celular", Order = 21)]
        public string ResponsavelCelular { get; set; }

        [JsonProperty(PropertyName = "responsavel_email", Order = 22)]
        public string ResponsavelEmail { get; set; }

        [JsonProperty(PropertyName = "responsavel_telefone", Order = 20)]
        public string ResponsavelTelefone { get; set; }

        [JsonProperty(PropertyName = "telefone_1", Order = 16, Required = Required.Always)]
        public string TelefonePrincipal { get; set; }

        [JsonProperty(PropertyName = "telefone_2", Order = 17)]
        public string TelefoneSecundario { get; set; }

        [JsonProperty(PropertyName = "tipo", Order = 1, Required = Required.Always)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "unidade", Order = 5, Required = Required.Always)]
        public string Unidade { get; set; }
    }
}
