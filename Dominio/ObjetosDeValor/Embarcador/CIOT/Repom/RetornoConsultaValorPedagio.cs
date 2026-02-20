using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("valor_vpr")]
    public class RetornoConsultaValorPedagio
    {
        [XmlElement("valor_total_vpr")]
        public string ValorTotalVPR { get; set; }
        
        [XmlElement("valor_vpr_ida")]
        public string ValorVPRIda { get; set; }
        
        [XmlElement("valor_vpr_volta")]
        public string ValorVPRVolta { get; set; }

        [XmlElement("percurso_descricao")]
        public string DescricaoPercurso { get; set; }
    }
}
