using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_viagem")]
    public class RetornoContratoFreteDadosViagem
    {
        [XmlElement("percurso_descricao")]
        public string DescricaoPercurso { get; set; }

        [XmlElement("operacao_descricao")]
        public string DescricaoOperacao { get; set; }

        [XmlElement("liberador_descricao")]
        public string DescricaoLiberador { get; set; }

        [XmlElement("cartao_codigo")]
        public string CodigoCartao { get; set; }

        [XmlElement("valor_frete")]
        public string ValorFrete { get; set; }

        [XmlElement("valor_adiantamento")]
        public string ValorAdiantamento { get; set; }

        [XmlElement("vale_combustivel")]
        public string ValeCombustivel { get; set; }

        [XmlElement("valor_mercadoria")]
        public string ValorMercadoria { get; set; }

        [XmlElement("peso")]
        public string Peso { get; set; }

        [XmlElement("relacao_abastecimento")]
        public string RelacaoAbastecimento { get; set; }

        [XmlElement("relacao_quitacao")]
        public string RelacaoQuitacao { get; set; }

        [XmlElement("observacao")]
        public string Observacao { get; set; }

        [XmlElement("versao")]
        public string Versao { get; set; }

        [XmlElement("ciot")]
        public string CIOT { get; set; }

        [XmlElement("cartao_pedagio_codigo")]
        public string CodigoCartaoPedagio { get; set; }

        [XmlElement("valor_pedagio")]
        public string ValorPedagio { get; set; }

        /// <summary>
        /// 1 - Pendente
        /// 2 - Carregado
        /// 3 – Cancelado
        /// </summary>
        [XmlElement("status_pedagio")]
        public string StatusPedagio { get; set; }

        [XmlElement("agendamento_desembarque")]
        public string AgendamentoDesembarque { get; set; }
    }
}
