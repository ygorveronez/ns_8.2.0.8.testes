using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class EnderecoKMM
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public string IdExterno { get; set; }

        [JsonProperty(PropertyName = "cod_endereco", Required = Required.Default)]
        public int? CodEndereco { get; set; }

        [JsonProperty(PropertyName = "endereco_padrao", Required = Required.Default)]
        public bool EnderecoPadrao { get; set; }

        [JsonProperty(PropertyName = "cod_bacen", Required = Required.Default)]
        public int CodigoBacen { get; set; }

        [JsonProperty(PropertyName = "insere_municipio_ex", Required = Required.Default)]
        public int InsereMunicipioEx { get; set; }

        [JsonProperty(PropertyName = "municipio", Required = Required.Default)]
        public string Municipio { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string UF { get; set; }

        [JsonProperty(PropertyName = "municipio_id", Required = Required.Default)]
        public string MunicipioId { get; set; }

        [JsonProperty(PropertyName = "municipio_ibge", Required = Required.Default)]
        public string MunicipioIBGE { get; set; }

        [JsonProperty(PropertyName = "cep", Required = Required.Default)]
        public string CEP { get; set; }

        [JsonProperty(PropertyName = "logradouro", Required = Required.Default)]
        public string Logradouro { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "complemento", Required = Required.Default)]
        public string Complemento { get; set; }

        [JsonProperty(PropertyName = "bairro", Required = Required.Default)]
        public string Bairro { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public int Tipo { get; set; }

        [JsonProperty(PropertyName = "atividade_fiscal", Required = Required.Default)]
        public int AtividadeFiscal { get; set; }

        [JsonProperty(PropertyName = "inscricao_estadual", Required = Required.Default)]
        public string InscricaoEstadual { get; set; }

        [JsonProperty(PropertyName = "referencia_id", Required = Required.Default)]
        public string ReferenciaId { get; set; }
    }
}
