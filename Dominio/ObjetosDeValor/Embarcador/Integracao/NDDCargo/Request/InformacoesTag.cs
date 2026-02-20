using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class InformacoesTag
    {
        [XmlElement(ElementName = "codigoFornecedor")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores.FornecedorTag CodigoFornecedor { get; set; }
    }
}
