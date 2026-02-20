using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaRelatorioNavio
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public Enumeradores.SituacaoAtivoPesquisa Status { get; set; }
        public string CodigoIrin { get; set; }
        public string CodigoEmbarcacao { get; set; }
        public List<Enumeradores.TipoEmbarcacao> TipoEmbarcacao { get; set; }
        public string CodigoDocumentacao { get; set; }
        public string CodigoIMO { get; set; }
        public string NavioID { get; set; }

    }
}
