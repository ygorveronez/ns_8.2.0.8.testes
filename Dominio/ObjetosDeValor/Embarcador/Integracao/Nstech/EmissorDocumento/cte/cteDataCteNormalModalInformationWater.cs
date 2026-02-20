using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ZXing;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{

    public class cteDataCteNormalModalInformationWater
    {
        /// <summary>
        /// Waterway Transportation Modal Information(tag aquav)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModal modal { get; } = enumModal.water;

        /// <summary>
        /// Service Value Calculation Base for AFRMM(tag vPrest)
        /// </summary>
        public decimal afrmmBaseAmount { get; set; }

        /// <summary>
        /// AFRMM (Freight Surcharge for the Renewal of the Merchant Navy)(tag vAFRMM)
        /// </summary>
        public decimal afrmmAmount { get; set; }

        /// <summary>
        /// Ship Identification(tag xNavio)
        /// </summary>
        public string shipId { get; set; }

        /// <summary>
        /// Group of Information on Ferries(tag balsa)
        /// </summary>
        public List<string> ferryId { get; set; }

        /// <summary>
        /// Trip Number(tag nViag)
        /// </summary>
        public string journeyNumber { get; set; }

        /// <summary>
        /// <para>Tag direc</para>
        /// <para>north</para>
        /// <para>east</para>
        /// <para>south</para>
        /// <para>west</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDirection direction { get; set; }

        /// <summary>
        /// The IRIN of the ship must always be provided(tag irin)
        /// </summary>
        public string irin { get; set; }

        /// <summary>
        /// Group of Detailed Information on Containers(tag detCont)
        /// </summary>
        public List<cteDataCteNormalModalInformationWaterContainer> container { get; set; }

        /// <summary>
        /// Type of Navigation(tag tpNav)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumShipType shipType { get; set; }
    }

    public class cteDataCteNormalModalInformationWaterContainer
    {
        /// <summary>
        /// Container Identification(tag nCont)
        /// </summary>
        public string containerId { get; set; }

        /// <summary>
        /// Group of Information on the Container Seals and the Quantity of the Cargo(tag lacre)
        /// </summary>
        public List<cteDataCteNormalModalInformationWaterContainerSealNumber> sealNumber { get; set; }

        /// <summary>
        /// <para>Information on the Container Documents(tag infDoc)</para>
        /// <para>Objetos: cteDataCteNormalModalInformationWaterContainerDocumentNf, cteDataCteNormalModalInformationWaterContainerDocumentNfe</para>
        /// </summary>
        public object document { get; set; }
    }

    public class cteDataCteNormalModalInformationWaterContainerSealNumber
    {
        /// <summary>
        /// Tag nLacre
        /// </summary>
        public string sealNumber { get; set; }
    }

    /// <summary>
    /// Information on the Invoices(tag infNF)
    /// </summary>
    public class cteDataCteNormalModalInformationWaterContainerDocumentNf
    {
        /// <summary>
        /// nf
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumContainerDocumentType type { get; } = enumContainerDocumentType.nf;

        public List<cteDataCteNormalModalInformationWaterContainerDocumentNfDocument> documents { get; set; }
    }

    public class cteDataCteNormalModalInformationWaterContainerDocumentNfDocument
    {
        /// <summary>
        /// Tag serie
        /// </summary>
        public string serie { get; set; }

        /// <summary>
        /// Tag nDoc
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Pro-rated Unit of Measure(tag unidRat)
        /// </summary>
        public decimal proRatedUnit { get; set; }
    }

    /// <summary>
    /// Information on the Invoices(tag infNFe)
    /// </summary>
    public class cteDataCteNormalModalInformationWaterContainerDocumentNfe
    {
        /// <summary>
        /// nfe
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumContainerDocumentType type { get; } = enumContainerDocumentType.nfe;

        public List<cteDataCteNormalModalInformationWaterContainerDocumentNfeDocument> documents { get; set; }
    }

    public class cteDataCteNormalModalInformationWaterContainerDocumentNfeDocument
    {
        /// <summary>
        /// Access Key of the NF-e(tag chave)
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Pro-rated Unit of Measure(tag unidRat)
        /// </summary>
        public decimal proRatedUnit { get; set; }
    }
}