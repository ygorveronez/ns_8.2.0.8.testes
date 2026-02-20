using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    [XmlRoot(ElementName = "E1EDL20")]
    public sealed class DadosCarga
    {
        [XmlElement(ElementName = "VBELN")]
        public string Numero { get; set; }
        
        [XmlElement(ElementName = "VSBED")]
        public string ModeloVeicularCarga { get; set; }
        
        [XmlElement(ElementName = "VSTEL")]
        public string CodigoRemetenteFilial { get; set; }
        
        [XmlElement(ElementName = "NTGEW")]
        public decimal Peso { get; set; }
        
        [XmlElement(ElementName = "E1ADRM1")]
        public DadosCliente[] Clientes { get; set; }
        
        [XmlElement(ElementName = "E1EDL24")]
        public DadosPedidoProduto[] Produtos { get; set; }
        
        [XmlElement(ElementName = "E1EDL21")]
        public DadosTipoOperacao DadosTipoOperacao { get; set; }
        
        [XmlElement(ElementName = "E1EDL18")]
        public DadosInformacaoDeletar DadosInformacaoDeletar { get; set; }
    }
}
