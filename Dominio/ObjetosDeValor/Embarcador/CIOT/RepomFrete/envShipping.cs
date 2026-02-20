using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envShipping
    {
        public string Identifier { get; set; }
        public string Country { get; set; }
        public string HiredCountry { get; set; }
        public string HiredNationalId { get; set; }
        public string OperationIdentifier { get; set; }
        public Int64? CardNumber { get; set; }
        public Int64? VPRCardNumber { get; set; }
        public int? BrazilianRouteTraceCode { get; set; }
        public Int64? BrazilianRouteRouteCode { get; set; }
        public string IssueDate { get; set; }
        public decimal TotalFreightValue { get; set; }
        public decimal TotalLoadWeight { get; set; }
        public decimal TotalLoadValue { get; set; }
        public decimal AdvanceMoneyValue { get; set; }
        public string BranchCode { get; set; }

        /// <summary>
        /// Tipo de viagem emitida (1 - padrão ou 3 - Tac-Agregado)
        /// </summary>
        public string CIOTShippingType { get; set; }
        public string ExistingContractId { get; set; }
        public string LoadBrazilianNCM { get; set; }
        public int? LoadBrazilianANTTCodeType { get; set; }
        public string BrazilianIBGECodeSource { get; set; }
        public string BrazilianIBGECodeDestination { get; set; }
        public string BrazilianCEPSource { get; set; }
        public string BrazilianCEPDestination { get; set; }
        public int? TravelledDistance { get; set; }
        public VPR VPR { get; set; }
        public ShippingPayment ShippingPayment { get; set; }
        public List<Drivers> Drivers { get; set; }
        public List<Documents> Documents { get; set; }
        public List<Vehicles> Vehicles { get; set; }
        public List<AccountingAdjustments> AccountingAdjustments { get; set; }
        public ShippingReceiver ShippingReceiver { get; set; }
        public List<Taxes> Taxes { get; set; }
        //public ShippingFuel ShippingFuel { get; set; }
        //public ShippingInternationalRoute ShippingInternationalRoute { get; set; }
    }

    public class VPR
    {
        public bool Issue { get; set; }
        public bool VPROneWay { get; set; }
        public int VPRSuspendedAxleNumber { get; set; }
        public int? VPRReturnSuspendedAxleNumber { get; set; }
    }

    public class ShippingPayment
    {
        public string ExpectedDeliveryDate { get; set; }
        public string ExpectedDeliveryLocationType { get; set; }
    }

    public class Drivers
    {
        public string Country { get; set; }
        public string NationalId { get; set; }
        public bool Main { get; set; }
    }

    public class Documents
    {
        public string BranchCode { get; set; }
        public string DocumentType { get; set; }
        public string Series { get; set; }
        public string Number { get; set; }
        public string EletronicKey { get; set; }
    }

    public class Vehicles
    {
        public string Country { get; set; }
        public string LicensePlate { get; set; }
    }

    public class AccountingAdjustments
    {
        public string Identifier { get; set; }
        public decimal Value { get; set; }
    }

    public class ShippingReceiver
    {
        public string Country { get; set; }
        public string NationalId { get; set; }
        public string Name { get; set; }
        public string ReceiverType { get; set; }
    }

    public class Taxes
    {
        public string Type { get; set; }
        public decimal Value { get; set; }
    }

    public class ShippingFuel
    {
        public string FuelCardNumber { get; set; }
        public decimal? FuelValue { get; set; }
    }

    public class ShippingInternationalRoute
    {
        public string OriginCountry { get; set; }
        public string OriginCityCode { get; set; }
        public string OriginPostalCode { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationCityCode { get; set; }
        public string DestinationPostalCode { get; set; }
        public int? RouteCode { get; set; }
        public int? TraceCode { get; set; }
        public string CommodityCode { get; set; }
    }
}