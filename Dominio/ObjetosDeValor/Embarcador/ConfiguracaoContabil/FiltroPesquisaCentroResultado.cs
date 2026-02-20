using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public sealed class FiltroPesquisaCentroResultado
    {
        public double Remetente { get; set; }

        public double Destinatario { get; set; }

        public double Tomador { get; set; }

        public double Expedidor { get; set; }

        public double Recebedor { get; set; }

        public List<int> Empresas { get; set; }

        public int TipoOperacao { get; set; }

        public int TipoOcorrencia { get; set; }

        public int GrupoProduto { get; set; }

        public int CentroResultado { get; set; }

        public int GrupoTomador { get; set; }

        public int GrupoDestinatario { get; set; }

        public int GrupoRemetente { get; set; }

        public int CategoriaTomador { get; set; }

        public int CategoriaDestinatario { get; set; }

        public int CategoriaRemetente { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
    }
}
