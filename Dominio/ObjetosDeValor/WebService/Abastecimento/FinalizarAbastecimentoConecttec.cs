using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.WebService.Abastecimento
{
    public class FinalizarAbastecimentoConecttec
    {
        [JsonProperty("stationId")]
        public int? StationId { get; set; }

        [JsonProperty("fuelPointNumber")]
        public int? FuelPointNumber { get; set; }

        [JsonProperty("hoseNumber")]
        public int? HoseNumber { get; set; }

        [JsonProperty("authId")]
        public string AuthId { get; set; }

        [JsonProperty("reserveId")]
        public string ReserveId { get; set; }

        [JsonProperty("deliveryId")]
        public decimal? DeliveryId { get; set; }

        [JsonProperty("dateTime")]
        public DateTime? DateTime { get; set; }

        [JsonProperty("volume")]
        public decimal? Volume { get; set; }

        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("total")]
        public decimal? Total { get; set; }

        [JsonProperty("fuel")]
        public FuelFinalizar Fuel { get; set; }

        [JsonProperty("tank")]
        public TankFinalizar Tank { get; set; }

        [JsonProperty("attendant")]
        public CustomerFinalizar Attendant { get; set; }

        [JsonProperty("customer")]
        public AttendantFinalizar Customer { get; set; }
    }

    public class FuelFinalizar
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fuelType")]
        public int? FuelType { get; set; }
    }

    public class TankFinalizar
    {
        [JsonProperty("number")]
        public int? Number { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fuel")]
        public FuelFinalizar Fuel { get; set; }
    }

    public class AttendantFinalizar
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }
    }

    public class CustomerFinalizar
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("odometer")]
        public int? Odometer { get; set; }

        [JsonProperty("ETotStart")]
        public decimal? ETotStart { get; set; }

        [JsonProperty("ETotEnd")]
        public decimal? ETotEnd { get; set; }

        [JsonProperty("pricewithoutdiscount")]
        public decimal? PriceWithoutDiscount { get; set; }
    }

}
