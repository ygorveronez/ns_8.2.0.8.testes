using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataModalInformationRoad
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModalMDFe modal { get; } = enumModalMDFe.road;

        /// <summary>
        /// Tag infANTT
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfo regAgencyInfo { get; set; }

        /// <summary>
        /// Tag veicTracao
        /// </summary>
        public mdfeDataModalInformationRoadTractorVehicle tractorVehicle { get; set; }

        /// <summary>
        /// Tag veicReboque
        /// </summary>
        public List<mdfeDataModalInformationRoadTrailerVehicle> trailerVehicle { get; set; }

        /// <summary>
        /// Tag codAgPorto
        /// </summary>
        public string portSchedulingCode { get; set; }

        /// <summary>
        /// Tag lacRodo
        /// </summary>
        public List<string> seal { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfo
    {
        /// <summary>
        /// Tag RNTRC
        /// </summary>
        public string rntrc { get; set; }

        /// <summary>
        /// Tag infCIOT
        /// </summary>
        public List<mdfeDataModalInformationRoadRegAgencyInfoCiotData> ciotData { get; set; }

        /// <summary>
        /// Tag valePed
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfo tollVoucherInfo { get; set; }

        /// <summary>
        /// Tag infContratante
        /// </summary>
        public List<mdfeDataModalInformationRoadRegAgencyInfoContractorInfo> contractorInfo { get; set; }

        /// <summary>
        /// Tag infPag
        /// </summary>
        public List<mdfeDataModalInformationRoadRegAgencyInfoPaymentInfo> paymentInfo { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoCiotData
    {
        /// <summary>
        /// Tag CIOT
        /// </summary>
        public string ciot { get; set; }

        /// <summary>
        /// ciotIssuer
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoCiotDataCiotIssuer ciotIssuer { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoCiotDataCiotIssuer
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfo
    {
        /// <summary>
        /// Tag disp
        /// </summary>
        public List<mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfo> tollVoucherDeviceInfo { get; set; }

        /// <summary>
        /// Tag categCombVeic
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumVehicleCategory vehicleCategory { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfo
    {
        /// <summary>
        /// Tag CNPJForn
        /// </summary>
        public string supplier { get; set; }

        /// <summary>
        /// payer
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfoPayer payer { get; set; }

        /// <summary>
        /// Tag nCompra
        /// </summary>
        public string purchaseNumber { get; set; }

        /// <summary>
        /// Tag vValePed
        /// </summary>
        public decimal tollVoucherValue { get; set; }

        /// <summary>
        /// Tag tpValePed
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTollVoucherType tollVoucherType { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoTollVoucherInfoTollVoucherDeviceInfoPayer
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoContractorInfo
    {
        /// <summary>
        /// Tag xNome
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoContractorInfoContractorDocument contractorDocument { get; set; }

        /// <summary>
        /// Tag infContrato
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoContractorInfoContractDetail contractDetail { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoContractorInfoContractorDocument
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumParticipantType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string foreignId { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoContractorInfoContractDetail
    {
        /// <summary>
        /// Tag NroContrato
        /// </summary>
        public string contractNumber { get; set; }

        /// <summary>
        /// Tag vContratoGlobal
        /// </summary>
        public decimal contractValue { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoPaymentInfo
    {
        /// <summary>
        /// Tag xNome
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// personPayment
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPersonPayment personPayment { get; set; }

        /// <summary>
        /// Tag Comp
        /// </summary>
        public List<mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents> paymentComponents { get; set; }

        /// <summary>
        /// Tag vContrato
        /// </summary>
        public decimal contractValue { get; set; }

        /// <summary>
        /// Tag indAltoDesemp
        /// </summary>
        public string highPerformance { get; set; }

        /// <summary>
        /// Tag indPag
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumpaymentMethod paymentMethod { get; set; }

        /// <summary>
        /// Tag vAdiant
        /// </summary>
        public decimal advanceAmount { get; set; }

        /// <summary>
        /// Tag indAntecipaAdiant
        /// </summary>
        public string advancePrepaymentInd { get; set; }

        /// <summary>
        /// Tag infPrazo
        /// </summary>
        public List<mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoInstallmentDetail> installmentDetail { get; set; }

        /// <summary>
        /// Tag tpAntecip
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumPrepaymentType prepaymentType { get; set; }

        /// <summary>
        /// Tag infBanc
        /// </summary>
        public mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoBankDetail bankDetail { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPersonPayment
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumParticipantType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string foreignId { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoPaymentComponents
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enumPaymentComponentType type { get; set; }

        /// <summary>
        /// Tag vComp
        /// </summary>
        public decimal value { get; set; }

        /// <summary>
        /// Tag xComp
        /// </summary>
        public string description { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoInstallmentDetail
    {
        /// <summary>
        /// Tag nParcela
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Tag dVenc
        /// </summary>
        public string dueDate { get; set; }

        /// <summary>
        /// Tag vParcela
        /// </summary>
        public decimal value { get; set; }
    }

    public class mdfeDataModalInformationRoadRegAgencyInfoPaymentInfoBankDetail
    {
        /// <summary>
        /// type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumBankDetailType type { get; set; }

        /// <summary>
        /// Tag codBanco
        /// </summary>
        public string bankCode { get; set; }

        /// <summary>
        /// Tag codAgencia
        /// </summary>
        public string agencyCode { get; set; }

        /// <summary>
        /// Tag CNPJIPEF
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag PIX
        /// </summary>
        public string pix { get; set; }
    }

    public class mdfeDataModalInformationRoadTractorVehicle
    {
        /// <summary>
        /// Tag cInt
        /// </summary>
        public string internalCode { get; set; }

        /// <summary>
        /// Tag placa
        /// </summary>
        public string plate { get; set; }

        /// <summary>
        /// Tag RENAVAM
        /// </summary>
        public string renavam { get; set; }

        /// <summary>
        /// Tag tara
        /// </summary>
        public decimal tara { get; set; }

        /// <summary>
        /// Tag capKG
        /// </summary>
        public decimal kgCapacity { get; set; }

        /// <summary>
        /// Tag capM3
        /// </summary>
        public decimal m3Capacity { get; set; }

        /// <summary>
        /// Tag prop
        /// </summary>
        public mdfeDataModalInformationRoadOwner owner { get; set; }

        /// <summary>
        /// Tag condutor
        /// </summary>
        public List<mdfeDataModalInformationRoadDriver> driver { get; set; }

        /// <summary>
        /// Tag tpRod
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumWheelType wheelType { get; set; }

        /// <summary>
        /// Tag tpCar
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumBodyType bodyType { get; set; }

        /// <summary>
        /// Tag UF
        /// </summary>
        public string state { get; set; }
    }

    public class mdfeDataModalInformationRoadTrailerVehicle
    {
        /// <summary>
        /// Tag cInt
        /// </summary>
        public string internalCode { get; set; }

        /// <summary>
        /// Tag placa
        /// </summary>
        public string plate { get; set; }

        /// <summary>
        /// Tag RENAVAM
        /// </summary>
        public string renavam { get; set; }

        /// <summary>
        /// Tag tara
        /// </summary>
        public decimal tara { get; set; }

        /// <summary>
        /// Tag capM3
        /// </summary>
        public decimal m3Capacity { get; set; }

        /// <summary>
        /// Tag prop
        /// </summary>
        public mdfeDataModalInformationRoadOwner owner { get; set; }

        /// <summary>
        /// Tag tpCar
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumBodyType bodyType { get; set; }

        /// <summary>
        /// Tag UF
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Tag capKG
        /// </summary>
        public decimal kgCapacity { get; set; }
    }

    public class mdfeDataModalInformationRoadOwner
    {
        public mdfeDataModalInformationRoadOwnerDocumentType documentType { get; set; }

        /// <summary>
        /// Tag RNTRC
        /// </summary>
        public string rntrc { get; set; }

        /// <summary>
        /// Tag xNome
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tag IE
        /// </summary>
        public string stateRegistration { get; set; }

        /// <summary>
        /// Tag UF
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// tpProp
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumOwnerType ownerType { get; set; }
    }

    public class mdfeDataModalInformationRoadOwnerDocumentType
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }
    }

    public class mdfeDataModalInformationRoadDriver
    {
        /// <summary>
        /// individual - Tag CPF
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
    }
}
