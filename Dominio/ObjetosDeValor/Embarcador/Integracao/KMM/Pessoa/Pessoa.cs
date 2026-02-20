using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Pessoa
    {
        [JsonProperty(PropertyName = "cod_pessoa", Required = Required.Default)]
        public string CodPessoa { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Always)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "operation", Required = Required.Always)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "inscricao_estadual", Required = Required.Default)]
        public string InscricaoEstadual { get; set; }

        [JsonProperty(PropertyName = "cnae", Required = Required.Default)]
        public string CNAE { get; set; }

        [JsonProperty(PropertyName = "cod_contabil", Required = Required.Default)]
        public string CodContabil { get; set; }

        [JsonProperty(PropertyName = "data_cadastro", Required = Required.Default)]
        public string DataCadastro { get; set; }

        [JsonProperty(PropertyName = "observacoes", Required = Required.Default)]
        public string Observacoes { get; set; }

        [JsonProperty(PropertyName = "modalidades", Required = Required.Default)]
        public List<ModalidadePessoa> Modalidades { get; set; }

        [JsonProperty(PropertyName = "pessoa_fisica", Required = Required.Default)]
        public PessoaFisica PessoaFisica { get; set; }

        [JsonProperty(PropertyName = "pessoa_juridica", Required = Required.Default)]
        public PessoaJuridica PessoaJuridica { get; set; }

        [JsonProperty(PropertyName = "pessoa_estrangeira", Required = Required.Default)]
        public PessoaEstrangeira PessoaEstrangeira { get; set; }

        [JsonProperty(PropertyName = "documentos", Required = Required.Default)]
        public Documento Documentos { get; set; }

        [JsonProperty(PropertyName = "RNTRC", Required = Required.Default)]
        public RNTRC RNTRC { get; set; }

        [JsonProperty(PropertyName = "enderecos", Required = Required.Default)]
        public List<EnderecoKMM> Enderecos { get; set; }

        [JsonProperty(PropertyName = "telefones", Required = Required.Default)]
        public List<Telefone> Telefones{ get; set; }

        [JsonProperty(PropertyName = "emails", Required = Required.Default)]
        public List<Email> Emails { get; set; }

        [JsonProperty(PropertyName = "contas_bancarias", Required = Required.Default)]
        public List<ContaBancaria> ContasBancarias { get; set; }

        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public string IdExterno { get; set; }
    }
}
