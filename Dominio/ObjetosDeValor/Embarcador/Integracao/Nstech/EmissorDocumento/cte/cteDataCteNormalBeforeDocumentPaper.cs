using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataCteNormalBeforeDocumentPaper
    {
        /// <summary>
        /// Issuer of the previous document(tag emiDocAnt)
        /// </summary>
        public cteDataCteNormalBeforeDocumentIssuer issuer { get; set; }

        /// <summary>
        /// paper
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumBeforeDocumentType type { get; } = enumBeforeDocumentType.paper;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteDataCteNormalBeforeDocumentPaperDocument> documents { get; set; }
    }

    public class cteDataCteNormalBeforeDocumentPaperDocument
    {
        /// <summary>
        /// <para>Previous Transport Document Type(tag tpDoc)</para>
        /// <para>atre</para>
        /// <para>dta</para>
        /// <para>international_air_waybill</para>
        /// <para>international_waybill</para>
        /// <para>individual_bill_lading</para>
        /// <para>tif</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaperDocumentType documentType { get; set; }

        /// <summary>
        /// Fiscal Document Series(tag serie)
        /// </summary>
        public string serie { get; set; }

        /// <summary>
        /// Fiscal Document Subseries(tag subser)
        /// </summary>
        public string subSerie { get; set; }

        /// <summary>
        /// Fiscal Document Number(tag nDoc)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Issue date(tag dEmi)
        /// </summary>
        public string issueDate { get; set; }
    }
}