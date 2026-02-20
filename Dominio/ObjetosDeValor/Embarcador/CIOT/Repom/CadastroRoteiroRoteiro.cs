using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("roteiro")]
    public class CadastroRoteiroRoteiro
    {
        [XmlElement("roteiro_codigo_cliente")]
        public string CodigoRota { get; set; }

        [XmlElement("cidade_origem_ibge")]
        public string IBGEOrigem { get; set; }

        [XmlElement("cidade_origem_cep")]
        public string CEPOrigem { get; set; }

        [XmlElement("estado_origem")]
        public string EstadoOrigem { get; set; }

        [XmlElement("cidade_destino_ibge")]
        public string IBGEDestino { get; set; }

        [XmlElement("cidade_destino_cep")]
        public string CEPDestino { get; set; }

        [XmlElement("estado_destino")]
        public string EstadoDestino { get; set; }

        [XmlElement("usuario_nome")]
        public string NomeUsuario { get; set; }

        [XmlElement("usuario_telefone")]
        public string TelefoneUsuario { get; set; }

        [XmlElement("usuario_email")]
        public string EmailUsuario { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilial { get; set; }

        /// <summary>
        /// 0 - Contrato
        /// 1 - Controle de Viagem
        /// 4 - Viagem VPR
        /// </summary>
        [XmlElement("tipo_processo_transporte")]
        public string TipoProcessoTransporte { get; set; }

        [XmlElement("tempo_previsto_viagem")]
        public string TempoPrevistoViagem { get; set; }

        [XmlElement("tipo_local_quitacao")]
        public string TipoLocalQuitacao { get; set; }

        [XmlElement("codigo_local_quitacao")]
        public string CodigoLocalQuitacao { get; set; }

        /// <summary>
        /// 0 – Não / 1 - Sim 
        /// </summary>
        [XmlElement("ida_volta")]
        public string IdaVolta { get; set; }

        /// <summary>
        /// 0 – Não / 1 - Sim 
        /// </summary>
        [XmlElement("altera_roteiro")]
        public string AlteraRoteiro { get; set; }

        [XmlElement("observacao")]
        public string Observacao { get; set; }

        [XmlArray("vias")]
        public CadastroRoteiroRoteiroVia[] Vias { get; set; }

        [XmlElement("paradas_ibge")]
        public string Paradas { get; set; }
    }
}
