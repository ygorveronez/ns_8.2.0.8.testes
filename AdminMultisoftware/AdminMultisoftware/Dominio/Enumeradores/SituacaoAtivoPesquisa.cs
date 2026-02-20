using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Enumeradores
{
    public enum SituacaoAtivoPesquisa
    {
        Todos = 0,
        Ativo = 1,
        Inativo = 2
    }

    public static class SituacaoAtivoPesquisaHelper
    {
        public static string ObterDescricao(this SituacaoAtivoPesquisa situacao)
        {
            switch (situacao)
            {
                case SituacaoAtivoPesquisa.Ativo: return "Ativo";
                case SituacaoAtivoPesquisa.Inativo: return "Inativo";
                default: return string.Empty;
            }
        }
    }
}