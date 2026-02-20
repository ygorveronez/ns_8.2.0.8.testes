using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162
{
    public class Motorista
    {
        [JsonProperty(PropertyName = "company_id", Required = Required.Always)]
        public string CompanyID { get; set; }

        [JsonProperty(PropertyName = "cpf", Required = Required.Always)]
        public string CPF { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "cnh", Required = Required.Default)]
        public string CNH { get; set; }

        [JsonProperty(PropertyName = "cnh_uf", Required = Required.Default)]
        public string EstadoCNH { get; set; }

        [JsonProperty(PropertyName = "driver_category", Required = Required.Default)]
        public string CategoriaCNH { get; set; }

        [JsonProperty(PropertyName = "mother_name", Required = Required.Default)]
        public string NomeMae { get; set; }

        [JsonProperty(PropertyName = "father_name", Required = Required.Default)]
        public string NomePai { get; set; }

        [JsonProperty(PropertyName = "rg", Required = Required.Default)]
        public string RG { get; set; }

        [JsonProperty(PropertyName = "born_date", Required = Required.Default)]
        public string DataNascimento { get; set; }

        [JsonProperty(PropertyName = "issue_date", Required = Required.Default)]
        public string DataAdmissao { get; set; }

        [JsonProperty(PropertyName = "expiration_date", Required = Required.Default)]
        public string DataVencimentoCNH { get; set; }

        [JsonProperty(PropertyName = "first_license_date", Required = Required.Default)]
        public string DataCNH { get; set; }

        [JsonProperty(PropertyName = "email", Required = Required.Default)]
        public string Email { get; set; }

        //[JsonProperty(PropertyName = "ticket_points", Required = Required.Default)]
        //public string ticket_points { get; set; }

        //[JsonProperty(PropertyName = "cnh_cedula", Required = Required.Default)]
        //public string cnh_cedula { get; set; }

        [JsonProperty(PropertyName = "city", Required = Required.Default)]
        public string Cidade { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string Estado { get; set; }

        [JsonProperty(PropertyName = "zip_code", Required = Required.Default)]
        public string CEP { get; set; }

        [JsonProperty(PropertyName = "address", Required = Required.Default)]
        public string Endereco { get; set; }

        [JsonProperty(PropertyName = "address_number", Required = Required.Default)]
        public string EnderecoNumero { get; set; }

        [JsonProperty(PropertyName = "address_comp", Required = Required.Default)]
        public string EnderecoComplemento { get; set; }
    }
}
