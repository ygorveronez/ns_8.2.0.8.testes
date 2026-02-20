using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    [XmlRoot(ElementName = "correioslog")]

    public class ReturnSolicitaXmlPlp
    {
        [XmlElement(ElementName = "tipo_arquivo")]
        public string TipoArquivo { get; set; }

        [XmlElement(ElementName = "versao_arquivo")]
        public string VersaoArquivo { get; set; }

        [XmlElement(ElementName = "plp")]
        public PLP PLP { get; set; }

        [XmlElement(ElementName = "remetente")]
        public Remetente Remetente { get; set; }

        [XmlElement(ElementName = "forma_pagamento")]
        public string FormaPagamento { get; set; }

        [XmlElement(ElementName = "objeto_postal")]
        public ObjetoPostal[] ObjetoPostal { get; set; }
    }
}
