using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    /// <summary>
    /// Invoice Information(tag infNFe)
    /// </summary>
    public class cteDataCteNormalDocumentNfe
    {
        /// <summary>
        /// nfe
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentType type { get; } = enumDocumentType.nfe;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteDataCteNormalDocumentNfeDocuments> documents { get; set; }
    }

    public class cteDataCteNormalDocumentNfeDocuments
    {
        /// <summary>
        /// NF-e Access Key(tag chave)
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// PIN SUFRAMA(tag PIN)
        /// </summary>
        public string pin { get; set; }

        /// <summary>
        /// Expected Delivery Date(tag dPrev)
        /// </summary>
        public string deliveryDate { get; set; }

        /// <summary>
        /// <para>unit</para>
        /// <para>Objetos: cteDataCteNormalDocumentUnitCargo, cteDataCteNormalDocumentUnitTransport</para>
        /// </summary>
        public object unit { get; set; }
    }
}