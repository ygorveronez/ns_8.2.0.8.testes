using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    /// <summary>
    /// Invoice Information(tag infNF)
    /// </summary>
    public class cteDataCteNormalDocumentNf
    {
        /// <summary>
        /// nf
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentType type { get; } = enumDocumentType.nf;

        /// <summary>
        /// documents
        /// </summary>
        public List<cteDataCteNormalDocumentNfDocuments> documents { get; set; }
    }

    public class cteDataCteNormalDocumentNfDocuments
    {
        /// <summary>
        /// Invoice Manifest Number(tag nRoma)
        /// </summary>
        public string shipmentNumber { get; set; }

        /// <summary>
        /// Invoice Order Number(tag nPed)
        /// </summary>
        public string orderNumber { get; set; }

        /// <summary>
        /// <para>Invoice Model(tag mod)</para>
        /// <para>single</para>
        /// <para>rural_product</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentNfModel model { get; set; }

        /// <summary>
        /// Tag serie
        /// </summary>
        public string serie { get; set; }

        /// <summary>
        /// Tag nDoc
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Tag dEmi
        /// </summary>
        public string issueDate { get; set; }

        /// <summary>
        /// ICMS Taxable Base Value(tag vBC)
        /// </summary>
        public decimal taxBaseAmount { get; set; }

        /// <summary>
        /// Total ICMS Amount(tag vICMS)
        /// </summary>
        public decimal taxAmount { get; set; }

        /// <summary>
        /// ICMS ST Taxable Base Value(tag vBCST)
        /// </summary>
        public decimal taxSTBaseAmount { get; set; }

        /// <summary>
        /// Total ICMS ST Amount(tag vST)
        /// </summary>
        public decimal taxSTAmount { get; set; }

        /// <summary>
        /// Total Value of the Products(tag vProd)
        /// </summary>
        public decimal productAmount { get; set; }

        /// <summary>
        /// Total Invoice Amount(tag vNF)
        /// </summary>
        public decimal documentAmount { get; set; }

        /// <summary>
        /// Predominant CFOP(tag nCFOP)
        /// </summary>
        public string operationNatureCode { get; set; }

        /// <summary>
        /// Total Weight in Kg(tag nPeso)
        /// </summary>
        public decimal weight { get; set; }

        /// <summary>
        /// PIN SUFRAMA(tag PIN)
        /// </summary>
        public string pin { get; set; }

        /// <summary>
        /// Estimated Delivery Date(tag dPrev)
        /// </summary>
        public string deliveryDate { get; set; }

        /// <summary>
        /// <para>unit</para>
        /// <para>Objetos: cteDataCteNormalDocumentUnitCargo, cteDataCteNormalDocumentUnitTransport</para>
        /// </summary>
        public object unit { get; set; }
    }

    public class cteDataCteNormalDocumentUnitCargo
    {
        /// <summary>
        /// cargo
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentUnit type { get; } = enumDocumentUnit.cargo;

        /// <summary>
        /// Cargo Unit Information Containers/ULD/Others (tag infUnidCarga)
        /// </summary>
        public List<cteDataCteNormalDocumentUnitCargoUnit> cargoUnit { get; set; }
    }

    public class cteDataCteNormalDocumentUnitCargoUnit
    {
        /// <summary>
        /// <para>Cargo Unit Type(tag tpUnidCarga)</para>
        /// <para>container</para>
        /// <para>uld</para>
        /// <para>pallet</para>
        /// <para>others</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumCargoUnitType cargoUnitType { get; set; }

        /// <summary>
        /// Cargo Unit Identification(tag idUnidCarga)
        /// </summary>
        public string cargoUnitId { get; set; }

        /// <summary>
        /// Cargo Unit Seals(tag lacUnidCarga)
        /// </summary>
        public List<cteDataCteNormalDocumentUnitSealNumber> sealNumber { get; set; }

        /// <summary>
        /// Pro-rated Quantity Weight, Volume (tag qtdRat)
        /// </summary>
        public decimal proRatedQuantity { get; set; }
    }

    public class cteDataCteNormalDocumentUnitTransport
    {
        /// <summary>
        /// transport
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumDocumentUnit type { get; } = enumDocumentUnit.transport;

        /// <summary>
        /// Transport Unit Information Truck/Trailer/Wagon (tag infUnidTransp)
        /// </summary>
        public cteDataCteNormalDocumentUnitTransportUnit transportUnit { get; set; }
    }

    public class cteDataCteNormalDocumentUnitTransportUnit
    {
        /// <summary>
        /// <para>Transport Unit Type(tag tpUnidTransp)</para>
        /// <para>rodoviario_tracao</para>
        /// <para>rodoviario_reboque</para>
        /// <para>navio</para>
        /// <para>balsa</para>
        /// <para>aeronave</para>
        /// <para>vagao</para>
        /// <para>outros</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTransportUnitType transportUnitType { get; set; }

        /// <summary>
        /// Transport Unit Identification(tag idUnidTransp)
        /// </summary>
        public string transportUnitId { get; set; }

        /// <summary>
        /// Transport Unit Seals(tag lacUnidTransp)
        /// </summary>
        public List<cteDataCteNormalDocumentUnitSealNumber> sealNumber { get; set; }

        /// <summary>
        /// Cargo Unit Information Containers/ULD/Others(tag infUnidCarga)
        /// </summary>
        public List<cteDataCteNormalDocumentUnitCargoUnit> cargoUnit { get; set; }

        /// <summary>
        /// Pro-rated Quantity Weight, Volume (tag qtdRat)
        /// </summary>
        public decimal proRatedQuantity { get; set; }
    }

    public class cteDataCteNormalDocumentUnitSealNumber
    {
        /// <summary>
        /// Tag nLacre
        /// </summary>
        public string sealNumber { get; set; }
    }
}