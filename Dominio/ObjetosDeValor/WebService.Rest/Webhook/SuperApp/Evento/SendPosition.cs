using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento
{
    public class DataSendPosition : SuperAppData
    {
        public List<Position> Positions { get; set; }
    }

    public class Position
    {
        public bool IsMoving { get; set; }
        public DateTime PositionAt { get; set; }
        public double Odometer { get; set; }
        public Location Location { get; set; }
        public double Accuracy { get; set; }
        public double Speed { get; set; }
        public double SpeedAccuracy { get; set; }
        public double Heading { get; set; }
        public double HeadingAccuracy { get; set; }
        public double Altitude { get; set; }
        public double AltitudeAccuracy { get; set; }
        public Activity Activity { get; set; }
        public Battery Battery { get; set; }
        public string Event { get; set; }
        public string AppVersion { get; set; }
        public string ConfigAccuracy { get; set; }
        public dynamic Provider { get; set; }
        public bool Mock { get; set; }
        public double Code { get; set; }
        public bool Active { get; set; }
    }
    public class Activity
    {
        public string Type { get; set; }
        public double Confidence { get; set; }
    }
    public class Provider
    {
        public string Type { get; set; }
        public double Confidence { get; set; }
    }
    public class Battery
    {
        public bool IsCharging { get; set; }
        public decimal Level { get; set; }
    }
}
