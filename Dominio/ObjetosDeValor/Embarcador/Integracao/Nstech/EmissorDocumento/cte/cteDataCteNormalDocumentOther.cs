using Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    /// <summary>
    /// Information on Other Documents(tag infOutros)
    /// </summary>
    public class cteDataCteNormalDocumentOther
    {
        /// <summary>
        /// other
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentType type { get; } = enumDocumentType.other;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteDataCteNormalDocumentOtherDocuments> documents { get; set; }
    }

    public class cteDataCteNormalDocumentOtherDocuments
    {
        /// <summary>
        /// <para>Original Document Type(tag tpDoc)</para>
        /// <para>declaration</para>
        /// <para>pipeline</para>
        /// <para>cfe_sat</para>
        /// <para>nfce</para>
        /// <para>outros</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumOtherDocumentType documentType { get; set; }

        /// <summary>
        /// Document Description(tag descOutros)
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Document Description(tag nDoc)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Issue Date(tag dEmi)
        /// </summary>
        public string issueDate { get; set; }

        /// <summary>
        /// Document Value(tag vDocFisc)
        /// </summary>
        public decimal documentAmount { get; set; }

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