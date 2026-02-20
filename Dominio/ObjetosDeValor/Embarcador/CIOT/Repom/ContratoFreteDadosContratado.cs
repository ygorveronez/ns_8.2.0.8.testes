using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_contratado")]
    public class ContratoFreteDadosContratado
    {
        [XmlElement("contratado_cnpj_cpf")]
        public string CPFCNPJContratado { get; set; }

        [XmlElement("motorista_cpf")]
        public string CPFMotorista { get; set; }

        [XmlElement("cavalo_placa")]
        public string PlacaCavalo { get; set; }

        [XmlElement("carreta_placa")]
        public string PlacaCarreta { get; set; }

        [XmlElement("carreta_rntrc")]
        public string RNTRCCarreta { get; set; }

        [XmlElement("carreta_numero_eixos")]
        public string NumeroEixosCarreta { get; set; }

        [XmlElement("eixos_suspensos_ida")]
        public string EixosSuspensosIda { get; set; }

        [XmlElement("eixos_suspensos_volta")]
        public string EixosSuspensosVolta { get; set; }
    }
}
