namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot("transport_order", Namespace = "", IsNullable = false)]
    public class transport_order
    {
        public string id { get; set; }
        public string success { get; set; }
        public string answer { get; set; }
    }
}
