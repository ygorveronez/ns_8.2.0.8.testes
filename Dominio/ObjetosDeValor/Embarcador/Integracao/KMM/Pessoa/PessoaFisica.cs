using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class PessoaFisica
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "cpf", Required = Required.Default)]
        public string CPF { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "nome_social", Required = Required.Default)]
        public string NomeSocial { get; set; }

        [JsonProperty(PropertyName = "sexo", Required = Required.Default)]
        public string Sexo { get; set; }

        [JsonProperty(PropertyName = "data_nascimento", Required = Required.Default)]
        public string DataNascimento { get; set; }

        [JsonProperty(PropertyName = "estado_civil", Required = Required.Default)]
        public int EstadoCivil { get; set; }

        [JsonProperty(PropertyName = "raca_cor", Required = Required.Default)]
        public int RacaCor { get; set; }

        [JsonProperty(PropertyName = "grau_instrucao", Required = Required.Default)]
        public string GrauInstrucao { get; set; }

        [JsonProperty(PropertyName = "naturalidade", Required = Required.Default)]
        public Naturalidade Naturalidade { get; set; }

        [JsonProperty(PropertyName = "pais_nascimento_esocial", Required = Required.Default)]
        public string PaisNascimentoESocial { get; set; }

        [JsonProperty(PropertyName = "pais_nacionalidade_esocial", Required = Required.Default)]
        public string PaisNacionalidadeESocial { get; set; }

        [JsonProperty(PropertyName = "pais_nacionalidade_rais", Required = Required.Default)]
        public string PaisNacionalidadeRais { get; set; }

        [JsonProperty(PropertyName = "nome_pai", Required = Required.Default)]
        public string NomePai { get; set; }

        [JsonProperty(PropertyName = "cpf_pai", Required = Required.Default)]
        public string CpfPai { get; set; }

        [JsonProperty(PropertyName = "nome_mae", Required = Required.Default)]
        public string NomeMae { get; set; }

        [JsonProperty(PropertyName = "cpf_mae", Required = Required.Default)]
        public string CpfMae { get; set; }

        [JsonProperty(PropertyName = "possui_def_fisica", Required = Required.Default)]
        public bool PossuiDefFisica { get; set; }

        [JsonProperty(PropertyName = "possui_def_visual", Required = Required.Default)]
        public bool PossuiDefVisual { get; set; }

        [JsonProperty(PropertyName = "possui_def_auditiva", Required = Required.Default)]
        public bool PossuiDefAuditiva { get; set; }

        [JsonProperty(PropertyName = "possui_def_mental", Required = Required.Default)]
        public bool PossuiDefMental { get; set; }

        [JsonProperty(PropertyName = "possui_def_intelectual", Required = Required.Default)]
        public bool PossuiDefIntelectual { get; set; }

        [JsonProperty(PropertyName = "possui_def_reabilitado", Required = Required.Default)]
        public bool PossuiDefReabilitado { get; set; }

        [JsonProperty(PropertyName = "infos_deficiencia", Required = Required.Default)]
        public string InfosDeficiencia { get; set; }

    }
}
