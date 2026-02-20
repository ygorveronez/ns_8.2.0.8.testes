using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("configuracoes_viagem")]
    public class ContratoFreteConfiguracoesViagem
    {
        [XmlElement("data_saida")]
        public string DataSaida { get; set; }

        [XmlElement("hora_saida")]
        public string HoraSaida { get; set; }
    }
}
