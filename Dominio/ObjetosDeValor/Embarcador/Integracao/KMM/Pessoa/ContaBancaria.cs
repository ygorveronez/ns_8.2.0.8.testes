using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class ContaBancaria
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "id_externo", Required = Required.Default)]
        public string IdExterno { get; set; }

        [JsonProperty(PropertyName = "conta_id", Required = Required.Default)]
        public string ContaId { get; set; }

        [JsonProperty(PropertyName = "padrao", Required = Required.Default)]
        public bool Padrao { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public int Tipo { get; set; }

        [JsonProperty(PropertyName = "banco", Required = Required.Default)]
        public string Banco { get; set; }

        [JsonProperty(PropertyName = "agencia", Required = Required.Default)]
        public string Agencia { get; set; }

        [JsonProperty(PropertyName = "conta_ag_dv", Required = Required.Default)]
        public string ContaAgDv { get; set; }

        [JsonProperty(PropertyName = "conta", Required = Required.Default)]
        public string Conta { get; set; }

        [JsonProperty(PropertyName = "conta_dv", Required = Required.Default)]
        public string ContaDv { get; set; }

        [JsonProperty(PropertyName = "cod_pessoa_titular", Required = Required.Default)]
        public string CodPessoaTitular { get; set; }

        [JsonProperty(PropertyName = "operacao", Required = Required.Default)]
        public string Operacao { get; set; }

        [JsonProperty(PropertyName = "instituicao_id", Required = Required.Default)]
        public string InstituicaoId { get; set; }

        [JsonProperty(PropertyName = "instituicao_conta", Required = Required.Default)]
        public string InstituicaoConta { get; set; }
    }
}
