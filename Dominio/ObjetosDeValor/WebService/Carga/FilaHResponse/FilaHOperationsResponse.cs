using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class FilaHOperationsResponse
    {
        public int optype { get; set; }
        public List<FreightDocument> freightDocuments { get; set; }
        public List<CteProductDocument> cteProductDocuments { get; set; }
        public List<string> plantName { get; set; }
    }
    public class FreightDocument
    {
        public string comments { get; set; }
        public string dangerousLoad { get; set; }
        public string documentNumber { get; set; }
        public string documentNumberStatus { get; set; }
        public string tNumber { get; set; }
        public string carrierNumber { get; set; }
        public string carrierName { get; set; }
        public string carrierCNPJ { get; set; }
        public string UnileverReceivingLocation { get; set; }
        public string poBalance { get; set; }
        public string shipmentType { get; set; }
        public List<Partner> partners { get; set; }
        public List<VehicleType> vehicleTypes { get; set; }
    }

    public class Partner
    {
        public string partnerFunctionPARVW { get; set; }
        public string partnerFunctionLIFNR { get; set; }
        public string partnerFunctionKUNNR { get; set; }
        public string vendorCNPJ { get; set; }
        public string customerCNPJ { get; set; }
    }

    public class VehicleType
    {
        public string SAPVehicleTypeLoad { get; set; }
        public string destCity { get; set; }
    }

    public class Plant
    {
        public string plantName { get; set; }
    }

    public class Carrier
    {
        public string tNumber { get; set; }
        public List<Partner> partners { get; set; }
        public List<Location> locations { get; set; }
        public string carrierCNPJ { get; set; }
        public string carrierName { get; set; }
        public string occurNumber { get; set; }
        public string statusforDt { get; set; }
        public string shipmentType { get; set; }
        public string carrierNumber { get; set; }
        public string plateNumber01 { get; set; }
        public string plateNumber02 { get; set; }
        public string plateNumber03 { get; set; }
        public string plateNumber04 { get; set; }
        public string vehicleStateUnload { get; set; }
    }

    public class CteProductDocument
    {
        public int? serie { get; set; }
        public int? number { get; set; }
        public string comments { get; set; }
        public string netWeight { get; set; }
        public string issuerCNPJ { get; set; }
        public string issuerName { get; set; }
        public string grossWeight { get; set; }
        public string messageStatus { get; set; }
        public string operationType { get; set; }
        public string destinationCNPJ { get; set; }
        public string destinationName { get; set; }
        public string comexOperation_1 { get; set; }
        public string comexOperation_2 { get; set; }
        public List<ProductDocument> productDocuments { get; set; }
        public string complianceProcess { get; set; }
        public string documentAccessKey { get; set; }
        public string unileverOperation { get; set; }
        public int? documentAccessKeyStatus { get; set; }
    }

    public class Location
    {
        public string OccurDest { get; set; }
        public string OccurDestSAP { get; set; }
        public string SapVehicleTypeUnload { get; set; }
        public string ShipmentLegindicator { get; set; }
        public string UnileverLoadingLocation { get; set; }
        public string UniveleverReceivingLocation { get; set; }
    }

    public class Product
    {
        public List<ProductInfo> products { get; set; }
        public string productCode { get; set; }
        public string productWeight { get; set; }
        public string productcQuantity { get; set; }
        public string productDescription { get; set; }
        public string productMeasureUnit { get; set; }
        public string productWeightMeasureUnit { get; set; }
    }

    public class ProductInfo
    {
        public string supplierproductUOM { get; set; }
        public string unileverproductUOM { get; set; }
        public int supplierproductQtty { get; set; }
        public string unileverproductCode { get; set; }
        public decimal unileverproductQtty { get; set; }
        public string unileverproductDescription { get; set; }
        public string unileverPlantforMaterialCode { get; set; }
    }

    public class ProductDocument
    {
        public int? serie { get; set; }
        public int? number { get; set; }
        public List<Carrier> carriers { get; set; }
        public string comments { get; set; }
        public List<Product> products { get; set; }
        public string issuerCPF { get; set; }
        public string netWeight { get; set; }
        public string issuerCNPJ { get; set; }
        public string issuerName { get; set; }
        public double? carrierCNPJ { get; set; }
        public string carrierName { get; set; }
        public string grossWeight { get; set; }
        public List<string> sapcodeDest { get; set; }
        public string freightPaidby { get; set; }
        public string messageStatus { get; set; }
        public string operationType { get; set; }
        public List<string> sapcodeIssuer { get; set; }
        public string destinationCNPJ { get; set; }
        public string destinationName { get; set; }
        public string comexOperation_1 { get; set; }
        public double? comexOperation_2 { get; set; }
        public string issuerVendorName { get; set; }
        public string complianceProcess { get; set; }
        public string documentAccessKey { get; set; }
        public string unileverOperation { get; set; }
        public string fiscalpointofCheck { get; set; }
        public string issuerCustomerName { get; set; }
        public double? placeofdeliveryCNPJ { get; set; }
        public string placeofdeliveryName { get; set; }
        public string destinationVendorName { get; set; }
        public string intramunicipOperation { get; set; }
        public string destinationCustomerName { get; set; }
        public double? documentAccessKeyStatus { get; set; }
        public string placeofdeliveryVendorName { get; set; }
        public string placeofdeliveryCustomerName { get; set; }
    }
}
