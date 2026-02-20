using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    public class DefinicaoTratativasIrregularidade : RepositorioBase<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade>
    {
        #region Construtores

        public DefinicaoTratativasIrregularidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaDefinicaoTratativasIrregularidade filtrosPesquisa)
        {
            var consultaDefinicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade>();

            if (filtrosPesquisa.CodigoIrregularidade > 0)
                consultaDefinicao = consultaDefinicao.Where(obj => obj.Irregularidade.Codigo == filtrosPesquisa.CodigoIrregularidade);

            if (filtrosPesquisa.CodigoPortfolio > 0)
                consultaDefinicao = consultaDefinicao.Where(obj => obj.PortfolioModuloControle.Codigo == filtrosPesquisa.CodigoPortfolio);

            if (filtrosPesquisa.Situacao != SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivaPesquisa.Ativa)
                    consultaDefinicao = consultaDefinicao.Where(obj => obj.Ativa == true);
                else
                    consultaDefinicao = consultaDefinicao.Where(obj => obj.Ativa == false);
            }

            return consultaDefinicao;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade BuscarPorCodigo(int codigo)
        {
            var consultaDefinicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade>()
                .Where(obj => obj.Codigo == codigo);

            return consultaDefinicao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade> BuscarPorCodigos(List<int> codigos)
        {
            var consultaDefinicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return consultaDefinicao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaDefinicaoTratativasIrregularidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDefinicoes = Consultar(filtrosPesquisa);

            return ObterLista(consultaDefinicoes, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaDefinicaoTratativasIrregularidade filtrosPesquisa)
        {
            var consultaDefinicoes = Consultar(filtrosPesquisa);

            return consultaDefinicoes.Count();
        }

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle portfolio, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade)
        {
            var consultaDefinicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade>();

            return consultaDefinicao.Any(def => def.Irregularidade.Codigo == irregularidade.Codigo && def.PortfolioModuloControle.Codigo == portfolio.Codigo);
        }

        #endregion Métodos Públicos
    }
}
