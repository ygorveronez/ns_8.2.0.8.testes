using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("retorno_antt")]
    public class RetornoIntegracaoCadastroContratadoANTT
    {
        [XmlArray("contratado_erros_antt"), XmlArrayItem("contratado_erro_antt")]
        public RetornoIntegracaoCadastroContratadoANTTErro[] Erros { get; set; }

        [XmlElement("validado_antt")]
        public int? ValidadoANTT { get; set; }

        [XmlElement("rntrc_data_validade")]
        public string DataValidadeRNTRC { get; set; }

        [XmlElement("equiparado_tac")]
        public string EquiparadoTAC { get; set; }
    }
}
