using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class PessoaJuridica
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Default)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
        public string CNPJ { get; set; }

        [JsonProperty(PropertyName = "razao_social", Required = Required.Default)]
        public string RazaoSocial { get; set; }

        [JsonProperty(PropertyName = "razao_social_resumida", Required = Required.Default)]
        public string RazaoSocialResumida { get; set; }

        [JsonProperty(PropertyName = "nome_fantasia", Required = Required.Default)]
        public string NomeFantasia { get; set; }

        [JsonProperty(PropertyName = "inscricao_estadual", Required = Required.Default)]
        public string InscricaoEstadual { get; set; }

        [JsonProperty(PropertyName = "suframa", Required = Required.Default)]
        public string Suframa { get; set; }

        [JsonProperty(PropertyName = "regime_tributario", Required = Required.Default)]
        public int RegimeTributario { get; set; }

        [JsonProperty(PropertyName = "tipo_lucro", Required = Required.Default)]
        public int TipoLucro { get; set; }

        [JsonProperty(PropertyName = "atividade_fiscal", Required = Required.Default)]
        public int AtividadeFiscal { get; set; }

        [JsonProperty(PropertyName = "certificado_otm", Required = Required.Default)]
        public string CertificadoOTM { get; set; }

        [JsonProperty(PropertyName = "alvara", Required = Required.Default)]
        public string Alvara { get; set; }
}
}
