using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    public class PortfolioModuloControle : RepositorioBase<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>
    {
        #region Construtores

        public PortfolioModuloControle(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaPortfolioModuloControle filtrosPesquisa)
        {
            var consultaPortfolioModuloControle = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracao))
                consultaPortfolioModuloControle = consultaPortfolioModuloControle.Where(obj => obj.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaPortfolioModuloControle = consultaPortfolioModuloControle.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            return consultaPortfolioModuloControle;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle BuscarPorCodigo(int codigo)
        {
            var consultaPortfolioModuloControle = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>()
                .Where(obj => obj.Codigo == codigo);

            return consultaPortfolioModuloControle.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle> BuscarPorCodigos(List<int> codigos)
        {
            var consultaPortfolioModuloControle = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return consultaPortfolioModuloControle.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaPortfolioModuloControle filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPortfolioModuloControle = Consultar(filtrosPesquisa);

            return ObterLista(consultaPortfolioModuloControle, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaPortfolioModuloControle filtrosPesquisa)
        {
            var consultaPortfolioModuloControle = Consultar(filtrosPesquisa);

            return consultaPortfolioModuloControle.Count();
        }

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle portfolio)
        {
            var consultaPortfolio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>();

            consultaPortfolio = consultaPortfolio.Where(p => p.Descricao.Equals(portfolio.Descricao));
            consultaPortfolio = consultaPortfolio.Where(p => p.CodigoIntegracao.Equals(portfolio.CodigoIntegracao));

            return consultaPortfolio.Any();
        }

        #endregion Métodos Públicos
    }
}
