using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "E1EDL24")]
    public sealed class DadosPedidoProduto
    {
        [XmlElement(ElementName = "MATNR")]
        public string CodigoProduto { get; set; }
        
        [XmlElement(ElementName = "ARKTX")]
        public string Descricao { get; set; }
        
        [XmlElement(ElementName = "VGBEL")]
        public string NumeroPedido { get; set; }
        
        [XmlElement(ElementName = "LFIMG")]
        public decimal Volumes { get; set; }
        
        [XmlElement(ElementName = "BRGEW")]
        public decimal PesoBrutoProduto { get; set; }
    }
}
