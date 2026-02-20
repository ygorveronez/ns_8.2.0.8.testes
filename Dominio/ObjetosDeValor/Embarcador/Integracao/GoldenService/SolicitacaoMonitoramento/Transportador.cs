using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    [XmlRoot(ElementName = "TRANSPORTADOR")]
    public class Transportador
    {
        public Transportador() { }

        public Transportador(Dominio.Entidades.Empresa empresa)
        {
            Nome = empresa.RazaoSocial;
        }

        [XmlElement(ElementName = "NOME")]
        public string Nome { get; set; }
    }
}
