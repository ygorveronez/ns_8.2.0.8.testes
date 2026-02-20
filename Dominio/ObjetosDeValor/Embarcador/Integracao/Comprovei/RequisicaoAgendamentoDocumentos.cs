using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei
{
    public class RequisicaoAgendamentoDocumentos
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "annotation")]
        public string Annotation { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

    }
}
