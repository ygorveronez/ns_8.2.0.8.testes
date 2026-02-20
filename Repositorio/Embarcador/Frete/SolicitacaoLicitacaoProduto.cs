using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class SolicitacaoLicitacaoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto>
    {
        public SolicitacaoLicitacaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto> BuscarPorSolicitacaoLicitacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto>()
                .Where(obj => obj.SolicitacaoLicitacao.Codigo == codigo);

            return query
                .Fetch(obj => obj.ProdutoEmbarcador)
                .ToList();
        }

        #endregion

        #region Métodos Privados



        #endregion
    }
}
