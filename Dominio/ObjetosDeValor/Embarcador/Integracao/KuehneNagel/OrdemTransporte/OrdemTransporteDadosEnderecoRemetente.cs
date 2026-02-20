using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDadosEnderecoRemetente
    {
        [XmlElement(ElementName = "PartyInformation", Order = 1)]
        public DadosParticipante DadosPartida { get; set; }
    }
}
