using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frete
{
    public sealed class Licitacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Licitacao>
    {
        #region Construtores

        public Licitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.Licitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao filtrosPesquisa)
        {
            var consultaLicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Licitacao>();

            if (filtrosPesquisa.CodigoTabelaFrete > 0)
                consultaLicitacao = consultaLicitacao.Where(o => o.TabelaFrete.Codigo == filtrosPesquisa.CodigoTabelaFrete);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaLicitacao = consultaLicitacao.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Numero > 0)
                consultaLicitacao = consultaLicitacao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                var consultaLicitacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LicitacaoTransportador>()
                    .Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

                consultaLicitacao = consultaLicitacao.Where(o => o.LiberarTodosTransportadores || consultaLicitacaoTransportador.Where(t => t.Licitacao.Codigo == o.Codigo).Any());
            }

            return consultaLicitacao;
        }

        #endregion

        #region Métodos Públicos

        public int BuscarProximoNumero()
        {
            var consultaLicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Licitacao>();
            int? ultimoNumero = consultaLicitacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Frete.Licitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta paramentrosConsulta)
        {
            var consultaLicitacao = Consultar(filtrosPesquisa);

            return ObterLista(consultaLicitacao, paramentrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao filtrosPesquisa)
        {
            var consultaLicitacao = Consultar(filtrosPesquisa);

            return consultaLicitacao.Count();
        }

        #endregion
    }
}
