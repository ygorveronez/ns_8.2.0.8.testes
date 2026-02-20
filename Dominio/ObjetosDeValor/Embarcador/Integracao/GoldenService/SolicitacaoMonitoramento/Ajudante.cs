using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    [XmlRoot(ElementName = "AJUDANTE")]
    public class Ajudante
    {
        public Ajudante() { }

        public Ajudante(Dominio.Entidades.Usuario ajudante)
        {
            CPF = ajudante.CPF;
            Nome = ajudante.Nome;
            Tipo = ajudante.TipoMotorista == Enumeradores.TipoMotorista.Proprio ? "1" : "2";
        }

        [XmlElement(ElementName = "CPF")]
        public string CPF { get; set; }

        [XmlElement(ElementName = "NOME")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "TIPO")]
        public string Tipo { get; set; }
    }
}
