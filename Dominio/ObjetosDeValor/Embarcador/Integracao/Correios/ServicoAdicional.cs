using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Correios
{
    public class ServicoAdicional
    {
        [XmlElement("codigo_servico_adicional")]
        public string CodigosServicoAdicional { get; set; }

        [XmlElement(ElementName = "valor_declarado")]
        public string ValorDeclarado { get; set; }

    }
}
