using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_frete")]
    public class ContratoFreteDadosFrete
    {
        [XmlElement("valor_frete")]
        public string ValorFrete { get; set; }

        [XmlElement("valor_adiantamento")]
        public string ValorAdiantamento { get; set; }
    }
}
