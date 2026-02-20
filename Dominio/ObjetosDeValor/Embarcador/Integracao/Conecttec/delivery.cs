using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec
{
    public class Deliveries
    {
        public List<Delivery> Delivery { get; set; }
    }
    public class Delivery
    {
        public int StationId { get; set; }
        public int FuelPointNumber { get; set; }
        public int HoseNumber { get; set; }
        public int? AuthId { get; set; }
        public int? ReserveId { get; set; }
        public long DeliveryId { get; set; }
        public DateTime DateTime { get; set; }
        public double Volume { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public Fuel Fuel { get; set; }
        public Attendant Attendant { get; set; }
        public string ProviderName { get; set; }
        public int TankNumber { get; set; }
        public string TankName { get; set; }
        public double EtotStart { get; set; }
        public double EtotEnd { get; set; }
        public string ProviderId { get; set; }
    }
    public class Fuel
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int FuelType { get; set; }
        public string FuelTypeDescription { get; set; }
    }

    public class Attendant
    {
        public string Tag { get; set; }
        public string Name { get; set; }
        public int? Id { get; set; }
    }


}
