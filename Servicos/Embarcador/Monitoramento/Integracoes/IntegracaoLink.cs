using System.Xml.Serialization;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "data_hora")]
        public string Data_hora { get; set; }

        [XmlElement(ElementName = "data_hora_gravacao")]
        public string Data_hora_gravacao { get; set; }

        [XmlElement(ElementName = "latitude")]
        public double Latitude { get; set; }

        [XmlElement(ElementName = "longitude")]
        public double Longitude { get; set; }

        [XmlElement(ElementName = "velocidade")]
        public int Velocidade { get; set; }

        [XmlElement(ElementName = "posicao_memoria")]
        public int Posicao_memoria { get; set; }

        [XmlElement(ElementName = "numero_satelites")]
        public int Numero_satelites { get; set; }

        [XmlElement(ElementName = "panico")]
        public int Panico { get; set; }

        [XmlElement(ElementName = "ignicao")]
        public int Ignicao { get; set; }

        [XmlElement(ElementName = "entradas")]
        public int Entradas { get; set; }

        [XmlElement(ElementName = "saidas")]
        public int Saidas { get; set; }

        [XmlElement(ElementName = "horimetro")]
        public string Horimetro { get; set; }

        [XmlElement(ElementName = "odometro")]
        public string Odometro { get; set; }

        [XmlElement(ElementName = "logradouro")]
        public string Logradouro { get; set; }

        [XmlElement(ElementName = "motivo")]
        public int Motivo { get; set; }

        [XmlElement(ElementName = "contador")]
        public int Contador { get; set; }

        [XmlElement(ElementName = "placa")]
        public string Placa { get; set; }

        [XmlElement(ElementName = "numero_serie")]
        public string Numero_serie { get; set; }

    }

    [XmlRoot(ElementName = "resposta")]
    public class Resposta
    {
        [XmlElement(ElementName = "item")]
        public Item Item { get; set; }

    }

    [XmlRoot(ElementName = "return")]
    public class Return
    {
        [XmlElement(ElementName = "id")]
        public int Id { get; set; }

        [XmlElement(ElementName = "erro")]
        public string Erro { get; set; }

        //[XmlElement(ElementName = "resposta")]
        //public Resposta[] Resposta { get; set; }

    }

    [XmlRoot(ElementName = "recebeUltimasResponse", Namespace = "urn:wsPosicoes")]
    public class RecebeUltimasResponse
    {
        [XmlElement(ElementName = "return")]
        public Return Return { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "recebeUltimasResponse", Namespace = "urn:wsPosicoes")]
        public RecebeUltimasResponse RecebeUltimasResponse { get; set; }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false)]
    public class Envelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }

    }

}

