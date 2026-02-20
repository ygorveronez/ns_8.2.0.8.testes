using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor
{

    [XmlRoot(ElementName = "MensagemCB")]
    public class MensagemOnixSat
    {
        [XmlElement(ElementName = "mId")]
        public Int64 MId { get; set; }

        [XmlElement(ElementName = "veiID")]
        public int IdVeiculo { get; set; }

        [XmlElement(ElementName = "dt")]
        public DateTime DataVeiculo { get; set; }

        [XmlElement(ElementName = "dtInc")]
        public DateTime Data { get; set; }

        [XmlElement(ElementName = "lat")]
        public string LatStr { get; set; }
        public double Latitude { get { return LatStr?.ToDouble() ?? 0D; } }

        [XmlElement(ElementName = "lon")]
        public string LngStr { get; set; }
        public double Longitude { get { return LngStr?.ToDouble() ?? 0D; } }

        [XmlElement(ElementName = "vel")]
        public int Velocidade { get; set; }

        [XmlElement(ElementName = "evt5")]
        public bool Panico { get; set; }

        [XmlElement(ElementName = "evt37")]
        public bool ErroSensorTemperatura1 { get; set; }

        [XmlElement(ElementName = "evt38")]
        public bool ErroSensorTemperatura2 { get; set; }

        [XmlElement(ElementName = "evt39")]
        public bool ErroSensorTemperatura3 { get; set; }

        [XmlElement(ElementName = "st1")]
        public int? Temperatura1 { get; set; }

        [XmlElement(ElementName = "st2")]
        public int? Temperatura2 { get; set; }

        [XmlElement(ElementName = "st3")]
        public int? Temperatura3 { get; set; }

        [XmlElement(ElementName = "odm")]
        public int Hodometro { get; set; }

        [XmlElement(ElementName = "rpm")]
        public int RMP { get; set; }

        [XmlElement(ElementName = "lt")]
        public int LitrosNoTanque { get; set; }

        [XmlElement(ElementName = "bat")]
        public int Bateria { get; set; }

        [XmlElement(ElementName = "evt4")]
        public int Ignicao { get; set; }

        [XmlElement(ElementName = "mun")]
        public string Cidade { get; set; }

        [XmlElement(ElementName = "uf")]
        public string Estado { get; set; }

        [XmlElement(ElementName = "rua")]
        public string Endereco { get; set; }
    }

    [XmlRoot(ElementName = "ResponseMensagemCB")]
    public class ResponseMensagemOnixSat
    {
        [XmlElement(ElementName = "MensagemCB")]
        public List<MensagemOnixSat> Mensagem { get; set; }
    }

}
