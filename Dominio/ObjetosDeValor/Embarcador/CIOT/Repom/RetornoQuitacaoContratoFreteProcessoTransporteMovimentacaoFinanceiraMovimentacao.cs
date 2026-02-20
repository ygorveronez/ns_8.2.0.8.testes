using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("movimentacao")]
    public class RetornoQuitacaoContratoFreteProcessoTransporteMovimentacaoFinanceiraMovimentacao
    {
        [XmlElement("indice")]
        public string Indice { get; set; }

        [XmlElement("sistema_conta")]
        public string SistemaConta { get; set; }

        [XmlElement("descricao")]
        public string Descricao { get; set; }

        [XmlElement("valor")]
        public string Valor { get; set; }
        
        [XmlElement("tipo_movimento")]
        public string TipoMovimento { get; set; }

        [XmlElement("data")]
        public string Data { get; set; }
        
        [XmlElement("codigo_movimento")]
        public string CodigoMovimento { get; set; }

        [XmlElement("credito_sistema_conta")]
        public string CreditoSistemaConta { get; set; }

        [XmlElement("usuario_responsavel")]
        public string UsuarioResponsavel { get; set; }

        [XmlElement("codigo_movimento_repom")]
        public string CodigoMovimentoRepom { get; set; }
    }
}
