using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TransPanorama
{
    public class LastPosition
    {
        public string identifier { get; set; }
        public string complement { get; set; }
        public int deviceId { get; set; }
        public int icon { get; set; }
        public long serial { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string dateGps { get; set; }
        public bool memory { get; set; }
        public bool ignition { get; set; }
        public bool block { get; set; }
        public bool panic { get; set; }
        public int speed { get; set; }
    }

    public class ResponseLastPosition
    {
        public List<LastPosition> positions { get; set; }
    }
}