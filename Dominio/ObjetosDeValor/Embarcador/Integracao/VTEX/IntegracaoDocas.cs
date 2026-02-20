
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Docas
{
    public class Address
    {
        public string PostalCode { get; set; }
        public Country Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Neighborhood { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Reference { get; set; }
        public object Location { get; set; }
    }

    public class Country
    {
        public string Acronym { get; set; }
        public string Name { get; set; }
    }

    public class PickupStoreInfo
    {
        public bool IsPickupStore { get; set; }
        public object StoreId { get; set; }
        public string FriendlyName { get; set; }
        public Address Address { get; set; }
        public object AdditionalInfo { get; set; }
        public object DockId { get; set; }
        public object Distance { get; set; }
        public object BusinessHours { get; set; }
        public object PickupHolidays { get; set; }
        public object SellerId { get; set; }
        public bool IsThirdPartyPickup { get; set; }
    }

    public class DeliveryFromStoreInfo
    {
        public bool IsActice { get; set; }
        public double DeliveryRadius { get; set; }
        public double DeliveryFee { get; set; }
        public string DeliveryTime { get; set; }
        public double MaximumWeight { get; set; }
    }

    public class ShippingRateProvider
    {
        // Adicione as propriedades necessárias
    }

    public class DeliveryAgreement
    {
        // Adicione as propriedades necessárias
    }

    public class PickupStore
    {
        public PickupStoreInfo PickupStoreInfo { get; set; }
        public object StoreId { get; set; }
        public object PickupInStoreInfo { get; set; }
        public DeliveryFromStoreInfo DeliveryFromStoreInfo { get; set; }
        public Address Address { get; set; }
        public object Location { get; set; }
        public List<ShippingRateProvider> ShippingRatesProviders { get; set; }
        public List<DeliveryAgreement> DeliveryAgreementsIds { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string DockTimeFake { get; set; }
        public string TimeFakeOverhead { get; set; }
        public List<string> SalesChannels { get; set; }
        public List<string> FreightTableIds { get; set; }
        public string WmsEndPoint { get; set; }
        public bool IsActive { get; set; }
    }

}
