using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas
{
    public class OfertaCarga
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Status { get; set; }

        public string ExternalId { get; set; }

        public string Language { get; set; }

        public Broker Broker { get; set; }

        public string EndAt { get; set; }

        public LocationPoint Origin { get; set; }

        public LocationPoint Destination { get; set; }

        public List<LocationPoint> Stopovers { get; set; }

        public List<Contact> Contacts { get; set; }

        public NegotiationInfo NegotiationInfo { get; set; }

        public List<AdditionalInformation> AdditionalInformation { get; set; }
    }

    public class Broker
    {
        public Document Document { get; set; }

        public string Name { get; set; }
    }

    public class Document
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }

    public class LocationPoint
    {
        public Point Point { get; set; }
    }

    public class Point
    {
        public string CountryCode { get; set; }

        public string Country { get; set; }

        public string StateCode { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string CountyCode { get; set; }

        public string County { get; set; }

        public string Subdistrict { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string HouseNumber { get; set; }

        public string Label { get; set; }

        public string Complement { get; set; }
    }

    public class Contact
    {
        public List<ContactItem> Items { get; set; }
    }

    public class ContactItem
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }

    public class NegotiationInfo
    {
        public string Type { get; set; }

        public Price Price { get; set; }

        public bool WithVehicle { get; set; }

        public Audience Audience { get; set; }

        public List<VehicleConfiguration> VehicleConfigurations { get; set; }
    }

    public class Price
    {
        public decimal Value { get; set; }

        public string Currency { get; set; }
    }

    public class Audience
    {
        public Driver Driver { get; set; }

        public Geofence Geofence { get; set; }

        public string Segmentation { get; set; }
    }

    public class Driver
    {
        public Document Document { get; set; }
    }

    public class Geofence
    {
        public string Type { get; set; }

        public double[] Coordinates { get; set; } // [longitude, latitude]

        public GeofenceRadius Radius { get; set; }
    }

    public class GeofenceRadius
    {
        public double Value { get; set; }

        public string Unit { get; set; }
    }

    public class VehicleConfiguration
    {
        public string _id { get; set; }
    }

    public class AdditionalInformation
    {
        public LocalizedText Label { get; set; }

        public LocalizedText Description { get; set; }
    }

    public class LocalizedText
    {
        public string Pt { get; set; }

        public string En { get; set; }

        public string Es { get; set; }
    }

}