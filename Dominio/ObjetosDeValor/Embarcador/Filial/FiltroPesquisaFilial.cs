using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Filial
{
    public sealed class FiltroPesquisaFilial
    {
        public Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }

        public string CodigoIntegracao { get; set; }

        public string Descricao { get; set; }

        public string DescricaoOuCodigoIntegracao { get; set; }

        public List<int> ListaCodigoFilialPermitidas { get; set; }
        public List<int> ListaCodigoFiliaisVendasPermitidas { get; set; }

        public bool SomenteFiliaisComSolicitacaoDeGas { get; set; }

        public bool SomenteLiberadasParaFilaCarregamento { get; set; }
    }
}
