using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class SolicitacaoLicitacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao>
    {
        public SolicitacaoLicitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao>();
            int? ultimoNumero = query.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaSolicitacaoLicitacao filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaSolicitacaoLicitacao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaSolicitacaoLicitacao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacao>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= filtrosPesquisa.DataInicio);

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= filtrosPesquisa.DataFim);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoLicitacao.Todos)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.Numero > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoFuncionarioSolicitante > 0)
                result = result.Where(obj => obj.Usuario.Codigo == filtrosPesquisa.CodigoFuncionarioSolicitante);

            if (filtrosPesquisa.CodigoFuncionarioCotacao > 0)
                result = result.Where(obj => obj.UsuarioCotacao.Codigo == filtrosPesquisa.CodigoFuncionarioCotacao);

            return result;
        }

        #endregion
    }
}
