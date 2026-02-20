using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class FiltroPesquisaRelatorioGrupoPessoas
    {
        public List<TipoGrupoPessoas> TipoGrupo { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public int Bloqueado { get; set; }
    }
}
