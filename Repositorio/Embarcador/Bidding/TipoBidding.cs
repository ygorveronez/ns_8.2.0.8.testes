using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Bidding
{
    public class TipoBidding : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.TipoBidding>
    {
        public TipoBidding(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoBidding(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Bidding.TipoBidding> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBidding filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBidding filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.TipoBidding> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBidding>();

            var result = from obj in query where codigosIntegracao.Contains(obj.CodigoIntegracao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.TipoBidding BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBidding>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Bidding.TipoBidding> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBidding filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBidding>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            return result;
        }

        #endregion Métodos Privados
    }
}
