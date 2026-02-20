using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API
{
    [XmlRoot(ElementName = "situacaoCartorial")]
    public class BuscarDpaCartaCpfResponse
    {
        public int cartorialId { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public string transportadora { get; set; }
        public string status { get; set; }
    }
}
