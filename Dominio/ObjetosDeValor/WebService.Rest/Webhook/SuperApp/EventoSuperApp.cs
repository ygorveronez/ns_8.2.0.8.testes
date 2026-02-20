using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp
{
    /// <summary>
    /// Objeto criado a partir da definição do Webhook da Trizy: https://api.trizy.com.br/api-trizy-app/webhook/sample/api-docs#tag/hooks
    /// </summary>
    #region Classe padrão recebimento Webhook

    public class EventoSuperApp
    {
        public string Event { get; set; }
        public DataPadrao Data { get; set; }
    }

    public class DataPadrao : SuperAppData
    {
        //EventoEventsSubmit
        public Event Event { get; set; }

        //EventoDeliveryReceiptCreate
        public StoppingPointDocument StoppingPointDocument { get; set; }
        public bool Checked { get; set; }
        public List<Evidence> Evidences { get; set; }
        public Location Location { get; set; }

        //EventoDriverReceiptCreate
        public Response Response { get; set; }
        public string DriverReceipt { get; set; }

        //EventoChatSendMessage
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public Chat Chat { get; set; }

        //EventoOccurrenceCreate
        public string _id { get; set; }
        public Category Category { get; set; }
        public Owner Owner { get; set; }

        // DriverFreightContactCreate
        public Freight Freight { get; set; }

        public VehicleComposition VehicleComposition { get; set; }

        public List<StoppingPointDocumentItem> StoppingPointDocumentItems { get; set; }

        // Partial Delivery e Not Delivery
        public Category Reason { get; set; }

    }
    #endregion

    #region Classes específicas para cada Webhook

    public class EventoEventsSubmit
    {
        public string Event { get; set; }
        public Evento.DataEventsSubmit Data { get; set; }
    }

    public class EventoDeliveryReceiptCreate
    {
        public string Event { get; set; }
        public Evento.DataDeliveryReceiptCreate Data { get; set; }
    }

    public class EventoDriverReceiptCreate
    {
        public string Event { get; set; }
        public Evento.DataDriverReceiptCreate Data { get; set; }
    }

    public class EventoChatSendMessage
    {
        public string Event { get; set; }
        public Evento.DataChatSendMessage Data { get; set; }
    }

    public class EventoOccurrenceCreate
    {
        public string Event { get; set; }
        public Evento.DataOccurrenceCreate Data { get; set; }
    }

    public class EventoSendPosition
    {
        public string Event { get; set; }
        public Evento.DataSendPosition Data { get; set; }
    }

    public class EventoSalvarDevolucao
    {
        public string Event { get; set; }
        public Evento.SalvarDevolucao Data { get; set; }
    }

    #endregion

    #region Classes compartilhadas entre eventos

    public class SuperAppData
    {
        public Driver Driver { get; set; }
        public Travel Travel { get; set; }
        public StoppingPoint StoppingPoint { get; set; }
        public string Client { get; set; }
        public DateTime SendAt { get; set; }
    }

    public class Driver
    {
        public Document Document { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string CellPhone { get; set; }
    }

    public class Document
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class Travel
    {
        public string _id { get; set; }
        public string ExternalID { get; set; }
        public ExternalInfo ExternalInfo { get; set; }
    }

    public class ExternalInfo
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public List<string> Tags { get; set; }
    }

    public class StoppingPoint
    {
        public string _id { get; set; }
        public string ExternalId { get; set; }
        public string Operation { get; set; }
    }

    public class Location
    {
        [Newtonsoft.Json.JsonProperty("type")]
        public string? Type { get; set; }
        /// <summary>
        /// Coordinates in GeoJSON. Format: [longitude, latitude].
        /// </summary>
        [Newtonsoft.Json.JsonProperty("coordinates")]
        public List<double>? Coordinates { get; set; }
    }

    public class Evidence
    {
        public string _id { get; set; }
        public string Type { get; set; }
        public string ExternalId { get; set; }
        public List<string> Values { get; set; }
        public List<Steps> Steps { get; set; }
    }

    public class Steps
    {
        public string _id { get; set; }
        public string Type { get; set; }
        public List<string> Values { get; set; }
    }

    public class Event
    {
        public string _id { get; set; }
        public string ExternalId { get; set; }
        public string Type { get; set; }
        public DateTime PreviousAt { get; set; }
        public DateTime RealizedAt { get; set; }
        public DateTime SynchronizedAt { get; set; }
        public Location Location { get; set; }
    }

    public class Chat
    {
        public string _id { get; set; }
        public string ExternalId { get; set; }
    }

    public class Freight
    {
        public string _id { get; set; }

        public string ExternalId { get; set; }
    }

    public class VehicleComposition
    {
        public bool IsMain { get; set; }
        public DateTime EngagedAt { get; set; }
        public long Code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Active { get; set; }
        public int __v { get; set; }
        public string _id { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public string VehicleBody { get; set; }
        public string VehicleConfiguration { get; set; }
    }

    public class Vehicle
    {
        public string City { get; set; }
        public string StateCode { get; set; }
        public string CountryCode { get; set; }
        public VehicleOwner Owner { get; set; }
        public List<string> FilesUrls { get; set; }
        public NationalRegistryOfRoadCargoTransporter NationalRegistryOfRoadCargoTransporter { get; set; }
        public Licensing Licensing { get; set; }
        public TrustInfo TrustInfo { get; set; }
        public long Code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Active { get; set; }
        public int __v { get; set; }
        public string LicensePlate { get; set; }
        public Axles Axles { get; set; }
        public string NationalRegister { get; set; }
        public string VehicleIdentificationNumber { get; set; }
        public string Manufacturer { get; set; }
        public int ManufactureYear { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string Color { get; set; }
        public string _id { get; set; }
    }

    public class VehicleOwner
    {
        public Document Document { get; set; }
        public string Name { get; set; }
        public string telephone { get; set; }
        public string socialIntegrationProgramNumber { get; set; }
        public Address address { get; set; }
    }


    public class Address
    {
        public string countryCode { get; set; }
        public string country { get; set; }
        public string stateCode { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string street { get; set; }
        public string postalCode { get; set; }
        public string block { get; set; }
        public string houseNumber { get; set; }
        public string building { get; set; }
        public string label { get; set; }
    }


    public class NationalRegistryOfRoadCargoTransporter
    {
        public string Number { get; set; }
    }

    public class Licensing
    {
        public int ExerciseYear { get; set; }
        public string AnnualLicensingCertificateSecurityCode { get; set; }
    }

    public class TrustInfo
    {
        public bool Trusted { get; set; }
        public string DocumentValidator { get; set; }
        public DateTime TrustedAt { get; set; }
        public string TrustedBy { get; set; }
    }

    public class Axles
    {
        public int Count { get; set; }
    }

    public class StoppingPointDocumentItem
    {
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DadosProduto ValueDelivered { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DadosProduto ValueTotal { get; set; }
        /** Valor será calculado pela Multi */
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DadosProduto? ValorDevolvidoCalculado { get; set; }
        public string ExternalId { get; set; }
    }

    #endregion
}