using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("veiculo")]
    public class Veiculo
    {
        [XmlElement("contratado_cnpj_cpf")]
        public string CPFCNPJContratado { get; set; }

        [XmlElement("placa")]
        public string Placa { get; set; }

        [XmlElement("rntrc_veiculo")]
        public string RNTRC { get; set; }

        [XmlElement("marca")]
        public string Marca { get; set; }

        [XmlElement("modelo")]
        public string Modelo { get; set; }

        [XmlElement("numero_eixos")]
        public string NumeroEixos { get; set; }

        [XmlElement("ano")]
        public string Ano { get; set; }

        [XmlElement("cor")]
        public string Cor { get; set; }

        [XmlElement("numero_chassis")]
        public string NumeroChassis { get; set; }

        [XmlElement("renavam")]
        public string RENAVAM { get; set; }

        [XmlElement("cidade")]
        public string Cidade { get; set; }

        [XmlElement("estado")]
        public string Estado { get; set; }

        [XmlElement("rastreador")]
        public string Rastreador { get; set; }

        [XmlElement("rastreador_codigo")]
        public string CodigoRastreador { get; set; }

        /// <summary>
        /// 0 - Não
        /// 1 - Sim
        /// </summary>
        [XmlElement("semi_reboque")]
        public string SemiReboque { get; set; }

        [XmlElement("peso")]
        public string Peso { get; set; }

        [XmlElement("volume")]
        public string Volume { get; set; }

        /// <summary>
        /// 0 - INDEFINIDO 1 – TRACTOR 
        /// 2 – REMOLQUE 
        /// 3 - CARRETA 5 EIXOS 
        /// 4 - CARRETA 4 EIXOS 
        /// 5 - TRUCK 
        /// 6 - TOCO 
        /// 7 - 3/4 
        /// 8 - VAN 
        /// 9 - UTILITARIO 
        /// 10 – VUC 
        /// 11 - CARRETA 6 EIXOS
        /// </summary>
        [XmlElement("tipo_veiculo")]
        public string TipoVeiculo { get; set; }
    }
}
