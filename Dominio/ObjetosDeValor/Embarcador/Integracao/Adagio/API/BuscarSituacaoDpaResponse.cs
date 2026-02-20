using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API
{
    [XmlRoot(ElementName = "situacaoChecklist")]
    public class BuscarSituacaoDpaResponse
    {
        public int checklistId { get; set; }
        public string dataAvaliacao { get; set; }
        public DateTime dataAvaliacaoDT { get { return DateTime.Parse(dataAvaliacao); } }
        public string placa { get; set; }
        public string unidade { get; set; }
        public string transportadora { get; set; }
        public string status { get; set; }
    }
}
