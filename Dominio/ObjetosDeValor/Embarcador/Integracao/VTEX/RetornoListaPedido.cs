using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX
{

    // Pedido myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class RetornoListaPedidos
    {
        public List<Pedido> list;
        public Paging paging;
    }

    public class Paging
    {
        public int pages { get; set; }
        public int currentPage { get; set; }

    }

    public class Pedido
    {
        public string affiliateId { get; set; }
        public bool allowCancellation { get; set; }
        public bool allowEdition { get; set; }
        public DateTime? authorizedDate { get; set; }
        public object callCenterOperatorData { get; set; }
        public object cancelReason { get; set; }
        public object cancellationData { get; set; }
        public object changesAttachment { get; set; }
        public object checkedInPickupPointId { get; set; }
        public ClientProfileData clientProfileData { get; set; }
        public object commercialConditionData { get; set; }
        public DateTime creationDate { get; set; }
        public object customData { get; set; }
        public string followUpEmail { get; set; }
        public object giftRegistryData { get; set; }
        public string hostname { get; set; }
        public InvoiceData invoiceData { get; set; }
        public DateTime? invoicedDate { get; set; }
        public bool isCheckedIn { get; set; }
        public bool isCompleted { get; set; }
        public List<Item> items { get; set; }
        public DateTime lastChange { get; set; }
        public object lastMessage { get; set; }
        public object marketingData { get; set; }
        public Marketplace marketplace { get; set; }
        public List<object> marketplaceItems { get; set; }
        public string marketplaceOrderId { get; set; }
        public string marketplaceServicesEndpoint { get; set; }
        public object merchantName { get; set; }
        public object openTextField { get; set; }
        public string orderFormId { get; set; }
        public string orderGroup { get; set; }
        public string orderId { get; set; }
        public string sellerOrderId { get; set; }
        public ShippingData shippingData { get; set; }
        public string status { get; set; }
        public string statusDescription { get; set; }
        public List<Seller> sellers { get; set; }
        public string salesChannel { get; set; }
    }

    public class ClientProfileData
    {
        public string id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string documentType { get; set; }
        public string document { get; set; }
        public string phone { get; set; }
        public object corporateName { get; set; }
        public object tradeName { get; set; }
        public string corporateDocument { get; set; }
        public object stateInscription { get; set; }
        public object corporatePhone { get; set; }
        public bool isCorporate { get; set; }
        public string userProfileId { get; set; }
        public object customerClass { get; set; }
    }

    public class Address
    {
        public string postalCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string neighborhood { get; set; }
        public object complement { get; set; }
        public object reference { get; set; }
        public List<double> geoCoordinates { get; set; }
        public string addressType { get; set; }
        public string receiverName { get; set; }
        public string addressId { get; set; }
    }

    public class InvoiceData
    {
        public Address address { get; set; }
        public object userPaymentInfo { get; set; }
    }

    public class Item
    {
        public string uniqueId { get; set; }
        public string id { get; set; }
        public string productId { get; set; }
        public string ean { get; set; }
        public string lockId { get; set; }
        public ItemAttachment itemAttachment { get; set; }
        public List<object> attachments { get; set; }
        public AdditionalInfo additionalInfo { get; set; }
        public int quantity { get; set; }
        public string seller { get; set; }
        public string name { get; set; }
        public string refId { get; set; }
        public int price { get; set; }
        public int listPrice { get; set; }
        public object manualPrice { get; set; }
        public string imageUrl { get; set; }
        public string detailUrl { get; set; }
        public string sellerSku { get; set; }
        public object priceValidUntil { get; set; }
        public int commission { get; set; }
        public int tax { get; set; }
        public object preSaleDate { get; set; }
        public string measurementUnit { get; set; }
        public double unitMultiplier { get; set; }
        public int sellingPrice { get; set; }
        public bool isGift { get; set; }
        public object shippingPrice { get; set; }
        public int rewardValue { get; set; }
        public int freightCommission { get; set; }
        public object priceDefinition { get; set; }
        public string taxCode { get; set; }
        public object parentItemIndex { get; set; }
        public object parentAssemblyBinding { get; set; }
        public object callCenterOperator { get; set; }
        public object serialNumbers { get; set; }
        public int? costPrice { get; set; }
    }

    public class ItemAttachment
    {
        public object name { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Dimension
    {
        public decimal cubicweight { get; set; }
        public decimal height { get; set; }
        public decimal length { get; set; }
        public decimal weight { get; set; }
        public decimal width { get; set; }
    }

    public class AdditionalInfo
    {
        public Dimension dimension { get; set; }
    }

    public class Marketplace
    {
        public string baseURL { get; set; }
        public object isCertified { get; set; }
        public string name { get; set; }
    }

    public class Package
    {
        public List<Item> items { get; set; }
        public string courier { get; set; }
        public string invoiceNumber { get; set; }
        public int invoiceValue { get; set; }
        public string invoiceUrl { get; set; }
        public DateTime issuanceDate { get; set; }
        public object trackingNumber { get; set; }
    }

    public class BillingAddress
    {
        public string postalCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public string neighborhood { get; set; }
        public object complement { get; set; }
        public object reference { get; set; }
        public List<double> geoCoordinates { get; set; }
    }

    public class Payment
    {
        public string id { get; set; }
        public string paymentSystem { get; set; }
        public string paymentSystemName { get; set; }
        public int value { get; set; }
        public int installments { get; set; }
        public int referenceValue { get; set; }
        public object cardHolder { get; set; }
        public object cardNumber { get; set; }
        public string firstDigits { get; set; }
        public string lastDigits { get; set; }
        public object cvv2 { get; set; }
        public object expireMonth { get; set; }
        public object expireYear { get; set; }
        public object url { get; set; }
        public object giftCardId { get; set; }
        public object giftCardName { get; set; }
        public object giftCardCaption { get; set; }
        public object redemptionCode { get; set; }
        public string group { get; set; }
        public object giftCardProvider { get; set; }
        public object giftCardAsDiscount { get; set; }
        public object koinUrl { get; set; }
        public string accountId { get; set; }
        public string parentAccountId { get; set; }
        public object bankIssuedInvoiceIdentificationNumber { get; set; }
        public object bankIssuedInvoiceIdentificationNumberFormatted { get; set; }
        public object bankIssuedInvoiceBarCodeNumber { get; set; }
        public object bankIssuedInvoiceBarCodeType { get; set; }
        public BillingAddress billingAddress { get; set; }
    }

    public class Transaction
    {
        public bool isActive { get; set; }
        public string transactionId { get; set; }
        public string merchantName { get; set; }
        public List<Payment> payments { get; set; }
    }

    public class PaymentData
    {
        public List<object> giftCards { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class RatesAndBenefitsData
    {
        public string id { get; set; }
        public List<object> rateAndBenefitsIdentifiers { get; set; }
    }

    public class Seller
    {
        public string id { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string fulfillmentEndpoint { get; set; }
    }

    public class PickupStoreInfo
    {
        public string additionalInfo { get; set; }
        public Address address { get; set; }
        public string dockId { get; set; }
        public string friendlyName { get; set; }
        public bool isPickupStore { get; set; }
    }

    public class Sla
    {
        public string id { get; set; }
        public string name { get; set; }
        public string shippingEstimate { get; set; }
        public object deliveryWindow { get; set; }
        public int price { get; set; }
        public string deliveryChannel { get; set; }
        public PickupStoreInfo pickupStoreInfo { get; set; }
        public object polygonName { get; set; }
        public string lockTTL { get; set; }
        public string pickupPointId { get; set; }
        public string transitTime { get; set; }
    }

    public class DeliveryId
    {
        public string courierId { get; set; }
        public string courierName { get; set; }
        public string dockId { get; set; }
        public int quantity { get; set; }
        public string warehouseId { get; set; }
        public string accountCarrierName { get; set; }
    }

    public class LogisticsInfo
    {
        public int itemIndex { get; set; }
        public string selectedSla { get; set; }
        public string lockTTL { get; set; }
        public int price { get; set; }
        public int listPrice { get; set; }
        public int sellingPrice { get; set; }
        public object deliveryWindow { get; set; }
        public string deliveryCompany { get; set; }
        public string shippingEstimate { get; set; }
        public DateTime? shippingEstimateDate { get; set; }
        public List<Sla> slas { get; set; }
        public List<string> shipsTo { get; set; }
        public List<DeliveryId> deliveryIds { get; set; }
        public string deliveryChannel { get; set; }
        public PickupStoreInfo pickupStoreInfo { get; set; }
        public string addressId { get; set; }
        public object polygonName { get; set; }
        public object pickupPointId { get; set; }
        public string transitTime { get; set; }
    }

    public class SelectedAddress
    {
        public string addressId { get; set; }
        public string addressType { get; set; }
        public string receiverName { get; set; }
        public string street { get; set; }
        public string number { get; set; }
        public object complement { get; set; }
        public string neighborhood { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public object reference { get; set; }
        public List<double> geoCoordinates { get; set; }
    }

    public class ShippingData
    {
        public string id { get; set; }
        public Address address { get; set; }
        public List<LogisticsInfo> logisticsInfo { get; set; }
        public object trackingHints { get; set; }
        public List<SelectedAddress> selectedAddresses { get; set; }
    }

    public class CurrencyFormatInfo
    {
        public int CurrencyDecimalDigits { get; set; }
        public string CurrencyDecimalSeparator { get; set; }
        public string CurrencyGroupSeparator { get; set; }
        public int CurrencyGroupSize { get; set; }
        public bool StartsWithCurrencySymbol { get; set; }
    }

    public class StorePreferencesData
    {
        public string countryCode { get; set; }
        public string currencyCode { get; set; }
        public CurrencyFormatInfo currencyFormatInfo { get; set; }
        public int currencyLocale { get; set; }
        public string currencySymbol { get; set; }
        public string timeZone { get; set; }
    }

    public class Total
    {
        public string id { get; set; }
        public string name { get; set; }
        public int value { get; set; }
    }

}
