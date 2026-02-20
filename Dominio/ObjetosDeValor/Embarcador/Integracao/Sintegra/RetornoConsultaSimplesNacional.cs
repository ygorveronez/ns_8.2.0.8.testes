using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Sintegra
{
    public class RetornoConsultaSimplesNacional
    {
        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
        public string CNPJ { get; set; }

        [JsonProperty(PropertyName = "cnpj_matriz", Required = Required.Default)]
        public string CNPJ_Matriz { get; set; }

        [JsonProperty(PropertyName = "nome_empresarial", Required = Required.Default)]
        public string NomeEmpresarial { get; set; }

        [JsonProperty(PropertyName = "situacao_simples_nacional", Required = Required.Default)]
        public string SituacaoSimplesNacional { get; set; }

        [JsonProperty(PropertyName = "situacao_simei", Required = Required.Default)]
        public string SituacaoSimei { get; set; }

        [JsonProperty(PropertyName = "situacao_simples_nacional_anterior", Required = Required.Default)]
        public string SituacaoSimplesNacionalAnterior { get; set; }

        [JsonProperty(PropertyName = "situacao_simei_anterior", Required = Required.Default)]
        public string SituacaoSimeiAnterior { get; set; }

        [JsonProperty(PropertyName = "agendamentos", Required = Required.Default)]
        public string Agendamentos { get; set; }

        [JsonProperty(PropertyName = "eventos_futuros_simples_nacional", Required = Required.Default)]
        public string EventosFuturosSimplesNacional { get; set; }

        [JsonProperty(PropertyName = "eventos_futuros_simples_simei", Required = Required.Default)]
        public string EventosFuturosSimplesSimei { get; set; }
    }
}
