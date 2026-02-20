using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retRouteData
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public RouteDetails Data { get; set; }
    }
    public class RouteDetails
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool Active { get; set; }
        public bool HaveReturn { get; set; }
        public bool Custom { get; set; }
        public string Id { get; set; }
        public int ShipperId { get; set; }
        public string GoingTripId { get; set; }
        public string GoingRouterTripId { get; set; }
        public string TripNickName { get; set; }
        public int TotalDistance { get; set; }
        public int TotalTolls { get; set; }
        public string Stops { get; set; }
        public string ReturnTripId { get; set; }
        public string ReturnRouterTripId { get; set; }
        public string ReturnStops { get; set; }
        public GoingTripDetails GoingTrip { get; set; }
    }

    public class GoingTripDetails
    {
        public OriginDestination Origin { get; set; }
        public OriginDestination Destination { get; set; }
        public string RouteType { get; set; }
        public List<Leg> Legs { get; set; }
        public string TripId { get; set; }
        public int TotalTolls { get; set; }
        public string RouterTripId { get; set; }
    }

    public class OriginDestination
    {
        public string Road { get; set; }
        public string ZipCode { get; set; }
        public string Uf { get; set; }
        public string City { get; set; }
        public string Label { get; set; }
    }

    public class Leg
    {
        public int Order { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public List<Vehicle> Vehicles { get; set; }
    }

    public class Vehicle
    {
        public string VehicleType { get; set; }
        public int TollsTotalCost { get; set; }
    }

}
