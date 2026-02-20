using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API
{
    [XmlRoot(ElementName = "situacaoCartorial")]
    public class BuscarDpaCartaPlacaResponse
    {
        public int cartorialId { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string placa { get; set; }
        public string unidade { get; set; }
        public string transportadora { get; set; }
        public string status { get; set; }
    }
}
