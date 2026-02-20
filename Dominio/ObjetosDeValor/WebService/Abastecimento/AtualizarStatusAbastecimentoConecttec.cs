using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.WebService.Abastecimento
{
    public class AtualizarStatusAbastecimentoConecttec
    {
        [JsonProperty("attendant")]
        public Attendant Attendant { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("fuel")]
        public Fuel Fuel { get; set; }

        [JsonProperty("tank")]
        public Tank Tank { get; set; }

        [JsonProperty("reserveId")]
        public string ReserveId { get; set; }

        [JsonProperty("reason")]
        public int? Reason { get; set; }

        [JsonProperty("fuelPointStatus")]
        public int? FuelPointStatus { get; set; }

    }

    public class Attendant
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("authId")]
        public string AuthId { get; set; }
    }

    public class Customer
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("odometer")]
        public int? Odometer { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }
    }

    public class Fuel
    {
        [JsonProperty("fuelType")]
        public int? FuelType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fuelPointNumber")]
        public int? FuelPointNumber { get; set; }

        [JsonProperty("fuelPointStatus")]
        public int? FuelPointStatus { get; set; }

        [JsonProperty("hoseNumber")]
        public List<int?> HoseNumber { get; set; }

        [JsonProperty("price")]
        public List<List<decimal?>> Price { get; set; }

        [JsonProperty("reason")]
        public int? Reason { get; set; }

        [JsonProperty("reserveId")]
        public string ReserveId { get; set; }

        [JsonProperty("stationId")]
        public int? StationId { get; set; }
    }

    public class Tank
    {
        [JsonProperty("fuel")]
        public FuelDetail Fuel { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("volume")]
        public int? Volume { get; set; }
    }

    public class FuelDetail
    {
        [JsonProperty("fuelType")]
        public int? FuelType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }


}
