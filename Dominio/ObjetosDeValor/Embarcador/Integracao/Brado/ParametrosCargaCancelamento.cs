using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class ParametrosCargaCancelamento
    {
        [JsonProperty("tipo_docto")]
        public string TipoDocumento { get; set; }

        [JsonProperty("tipo_cancelamento")]
        public string TipoCancelamento { get; set; }
        
        [JsonProperty("motivo_cancelamento")]
        public string MotivoCancelamento { get; set; }

        [JsonProperty("documentos")]
        public  List<DocumentosCargaCancelamento> Documentos { get; set; }

    }
}
