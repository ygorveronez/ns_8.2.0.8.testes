using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedDataDeliveryDocumentsNFe
    {
        /// <summary>
        /// nfe
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentTypeSimplified type { get; } = enumDocumentTypeSimplified.nfe;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteDataCteNormalDocumentNfeDocuments> documents { get; set; }
    }
}