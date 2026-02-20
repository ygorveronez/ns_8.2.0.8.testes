using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.JJ
{
    public class Chamado
    {
        [JsonProperty(PropertyName = "note", Required = Required.Default)]
        public string Nota { get; set; }

        [JsonProperty(PropertyName = "motive", Required = Required.Default)]
        public string Motivo { get; set; }

        [JsonProperty(PropertyName = "governmentInstitution", Required = Required.Default)]
        public string InstituicaoGovernamental { get; set; }

        [JsonProperty(PropertyName = "returnType", Required = Required.Default)]
        public int TipoDevolucao { get; set; }

        [JsonProperty(PropertyName = "justification", Required = Required.Default)]
        public string Justificativa { get; set; }

        [JsonProperty(PropertyName = "series", Required = Required.Default)]
        public string Serie { get; set; }

        [JsonProperty(PropertyName = "realMotive", Required = Required.Default)]
        public string RealMotivo { get; set; }

        [JsonProperty(PropertyName = "cnpjCustomer", Required = Required.Default)]
        public string CNPJDestinatario { get; set; }

        [JsonProperty(PropertyName = "typeCargo", Required = Required.Default)]
        public string TipoCarga { get; set; }

        [JsonProperty(PropertyName = "attachments", Required = Required.Default)]
        public List<ChamadoAnexo> Anexos { get; set; }

        [JsonProperty(PropertyName = "items", Required = Required.Default)]
        public List<ChamadoItem> Itens { get; set; }

        [JsonProperty(PropertyName = "protocol", Required = Required.Default)]
        public string Protocolo { get; set; }
    }
}
