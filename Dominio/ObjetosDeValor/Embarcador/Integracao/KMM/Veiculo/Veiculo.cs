using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Veiculo
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Always)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "veiculo_id", Required = Required.Default)]
        public string VeiculoId { get; set; }

        [JsonProperty(PropertyName = "frota", Required = Required.Default)]
        public string Frota { get; set; }

        [JsonProperty(PropertyName = "placa", Required = Required.Always)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "tipo_carroceria_id", Required = Required.Default)]
        public string TipoCarroceriaId { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string Uf { get; set; }

        [JsonProperty(PropertyName = "ano", Required = Required.Default)]
        public int? Ano { get; set; }

        [JsonProperty(PropertyName = "chassis", Required = Required.Default)]
        public string Chassis { get; set; }

        [JsonProperty(PropertyName = "renavan", Required = Required.Default)]
        public string Renavan { get; set; }

        [JsonProperty(PropertyName = "cor_predominante", Required = Required.Default)]
        public string CorPredominante { get; set; }

        [JsonProperty(PropertyName = "capacidade_tanque", Required = Required.Default)]
        public int? CapacidadeTanque { get; set; }

        [JsonProperty(PropertyName = "tara", Required = Required.Default)]
        public int? Tara { get; set; }

        [JsonProperty(PropertyName = "tag_ctf", Required = Required.Default)]
        public int? TagCtf { get; set; }

        [JsonProperty(PropertyName = "observacao", Required = Required.Default)]
        public string Observacao { get; set; }

        [JsonProperty(PropertyName = "atributos", Required = Required.Default)]
        public string Atributos { get; set; }

        [JsonProperty(PropertyName = "num_nota_id", Required = Required.Default)]
        public int? NumNotaId { get; set; }

        [JsonProperty(PropertyName = "num_contrato", Required = Required.Default)]
        public int? NumContrato { get; set; }

        [JsonProperty(PropertyName = "tipo_compra", Required = Required.Default)]
        public int? TipoCompra { get; set; }

        [JsonProperty(PropertyName = "data_compra", Required = Required.Default)]
        public string DataCompra { get; set; }

        [JsonProperty(PropertyName = "tag_sem_parar", Required = Required.Default)]
        public int? TagSemParar { get; set; }

        [JsonProperty(PropertyName = "data_inicio_sem_parar", Required = Required.Default)]
        public string DataInicioSemParar { get; set; }

        [JsonProperty(PropertyName = "marca_id", Required = Required.Default)]
        public string MarcaId { get; set; }

        [JsonProperty(PropertyName = "marca", Required = Required.Default)]
        public string Marca { get; set; }

        [JsonProperty(PropertyName = "modelo_id", Required = Required.Default)]
        public string ModeloId { get; set; }

        [JsonProperty(PropertyName = "modelo", Required = Required.Default)]
        public string Modelo { get; set; }

        [JsonProperty(PropertyName = "agrupamento_id", Required = Required.Default)]
        public string AgrupamentoId { get; set; }

        [JsonProperty(PropertyName = "cod_ibge", Required = Required.Default)]
        public int? CodIbge { get; set; }

        [JsonProperty(PropertyName = "cod_organizacional", Required = Required.Default)]
        public string CodOrganizacional { get; set; }

        [JsonProperty(PropertyName = "modalidade", Required = Required.Always)]
        public string Modalidade { get; set; }

        [JsonProperty(PropertyName = "municipio", Required = Required.Default)]
        public string Municipio { get; set; }

        [JsonProperty(PropertyName = "municipio_id", Required = Required.Default)]
        public string MunicipioId { get; set; }

        [JsonProperty(PropertyName = "descricao_carroceria", Required = Required.Default)]
        public string DescricaoCarroceria { get; set; }

        [JsonProperty(PropertyName = "proprietario", Required = Required.Default)]
        public Proprietario Proprietario { get; set; }

        [JsonProperty(PropertyName = "proprietario_documento", Required = Required.Default)]
        public ProprietarioDocumento ProprietarioDocumento { get; set; }
    }

    public class Proprietario
    {
        [JsonProperty(PropertyName = "cpf_cnpj", Required = Required.Default)]
        public string CpfCnpj { get; set; }

        [JsonProperty(PropertyName = "data_inicio", Required = Required.Default)]
        public string DataInicio { get; set; }

        [JsonProperty(PropertyName = "cod_pessoa", Required = Required.Default)]
        public string CodPessoa { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }
    }

    public class ProprietarioDocumento
    {
        [JsonProperty(PropertyName = "cpf_cnpj", Required = Required.Default)]
        public string CpfCnpj { get; set; }

        [JsonProperty(PropertyName = "data_inicio", Required = Required.Default)]
        public string DataInicio { get; set; }

        [JsonProperty(PropertyName = "cod_pessoa", Required = Required.Default)]
        public string CodPessoa { get; set; }

        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }
    }
}