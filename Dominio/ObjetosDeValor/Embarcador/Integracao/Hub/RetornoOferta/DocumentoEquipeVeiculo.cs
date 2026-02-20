using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class DocumentoEquipeVeiculo
    {
        [JsonProperty("documentNumber")]
        public string NumeroDocumento { get; set; }

        [JsonProperty("type")]
        public Tipo Tipo { get; set; }
    }
}
