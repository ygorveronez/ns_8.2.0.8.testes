using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingChecklistQuestionarioPadrao : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao>
    {
        public BiddingChecklistQuestionarioPadrao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao>()
                .Where(o => o.Codigo == codigo);

            return query
                .Fetch(o => o.Anexos)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingChecklistQuestionarioPadrao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingChecklistQuestionarioPadrao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao> BuscarPorTipoBidding(int codigoTipoBidding)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao>()
                .Where(o => o.TipoBidding.Codigo == codigoTipoBidding);

            return query
                .Fetch(o => o.Anexos)
                .ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingChecklistQuestionarioPadrao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            return result;
        }

        #endregion Métodos Privados
    }
}
