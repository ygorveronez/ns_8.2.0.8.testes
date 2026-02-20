using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Positron
{
    [XmlRoot(ElementName = "integration")]
    public class Integration
    {

        [XmlElement(ElementName = "id")]
        public long id { get; set; }

        [XmlElement(ElementName = "idVeic")]
        public long idVeic { get; set; }

        [XmlElement(ElementName = "vehicle")]
        public Vehicle vehicle { get; set; }

        [XmlElement(ElementName = "latitude")]
        public double latitude { get; set; }

        [XmlElement(ElementName = "longitude")]
        public double longitude { get; set; }

        [XmlElement(ElementName = "speed")]
        public int speed { get; set; }

        [XmlElement(ElementName = "altitude")]
        public double altitude { get; set; }

        [XmlElement(ElementName = "direction")]
        public double direction { get; set; }

        [XmlElement(ElementName = "ignition")]
        public bool ignition { get; set; }

        [XmlElement(ElementName = "validGPS")]
        public bool validGPS { get; set; }

        [XmlElement(ElementName = "satelital")]
        public bool satelital { get; set; }

        [XmlElement(ElementName = "bloqueio")]
        public bool bloqueio { get; set; }

        [XmlElement(ElementName = "moduleTime")]
        public DateTime moduleTime { get; set; }

        [XmlElement(ElementName = "serverTime")]
        public DateTime serverTime { get; set; }

        [XmlElement(ElementName = "hodometro")]
        public int hodometro { get; set; }

        [XmlElement(ElementName = "memory")]
        public bool memory { get; set; }

        [XmlElement(ElementName = "endereco")]
        public string endereco { get; set; }

        [XmlElement(ElementName = "codevt")]
        public int codevt { get; set; }

        [XmlElement(ElementName = "descevt")]
        public string descevt { get; set; }

    }

}
