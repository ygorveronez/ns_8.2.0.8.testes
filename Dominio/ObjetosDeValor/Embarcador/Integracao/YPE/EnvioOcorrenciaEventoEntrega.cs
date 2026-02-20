using Newtonsoft.Json;
using System.Collections.Generic;

//Devops: 443
namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YPE
{
    public class EnvioOcorrenciaEventoEntrega
    {
        [JsonProperty(PropertyName = "ZPORTO", Required = Required.Default)]
        public ZPorto zPorto { get; set; } = new ZPorto();
    }

    public class ZPorto
    {
        [JsonProperty(PropertyName = "danfe", Required = Required.Default)]
        public string danfe { get; set; }

        [JsonProperty(PropertyName = "cnpjemissaonf", Required = Required.Default)]
        public string cnpjemissaonf { get; set; }

        [JsonProperty(PropertyName = "serienf", Required = Required.Default)]
        public string serienf { get; set; }

        [JsonProperty(PropertyName = "numeronf", Required = Required.Default)]
        public string numeronf { get; set; }

        [JsonProperty(PropertyName = "etapas", Required = Required.Default)]
        public List<Etapa> etapas { get; set; } = new List<Etapa>();
    }

    public class Etapa
    {
        [JsonProperty(PropertyName = "idetapa", Required = Required.Default)]
        public string CodigoEntrega { get; set; }

        [JsonProperty(PropertyName = "tipoetapa", Required = Required.Default)]
        public string TipoOcorrencia { get; set; }

        [JsonProperty(PropertyName = "nomeetapa", Required = Required.Default)]
        public string DescricaoOcorrencia { get; set; }

        [JsonProperty(PropertyName = "dataetapa", Required = Required.Default)]
        public string Data { get; set; } //YYYY-MM-DD

        [JsonProperty(PropertyName = "horaetapa", Required = Required.Default)]
        public string Hora { get; set; }

        [JsonProperty(PropertyName = "viagem", Required = Required.Default)]
        public string CodigoCarga { get; set; }

        [JsonProperty(PropertyName = "codcd", Required = Required.Default)]
        public string CodigoCD { get; set; }

        [JsonProperty(PropertyName = "nomeporto", Required = Required.Default)]
        public string NomePorto { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public string Status { get; set; }
    }

}
