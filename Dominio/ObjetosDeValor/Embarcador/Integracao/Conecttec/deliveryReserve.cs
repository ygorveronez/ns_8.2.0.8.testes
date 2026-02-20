using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec
{
    public class deliveryReserve
    {
        public int StationId { get; set; }
        public int FuelPointNumber { get; set; }
        public string ProviderId { get; set; }
        public int ReserveTimeOut { get; set; }
    }
    public class StationReservation
    {
        public string ReserveId { get; set; }
        public string ProviderId { get; set; }
        public bool CanInvoice { get; set; }
        public bool CanReceipt { get; set; }
        public FuelPoints FuelPoints { get; set; }
    }


    public class Hose
    {
        public int HoseNumber { get; set; }
        public Fuel Fuel { get; set; }
    }

    public class FuelPoints
    {
        public bool Preset { get; set; }
        public bool Block { get; set; }
        public bool ConfigPrice { get; set; }
        public bool Card { get; set; }
        public bool Tag { get; set; }
        public bool Term { get; set; }
        public List<Hose> Hoses { get; set; }

    }
}
