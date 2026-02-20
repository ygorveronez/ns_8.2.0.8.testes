using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class SituacaoLancamentoDocumentoEntrada : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada>
    {
        #region Construtores

        public SituacaoLancamentoDocumentoEntrada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaSituacaoLancamentoDocumentoEntrada filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaSituacaoLancamentoDocumentoEntrada filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaSituacaoLancamentoDocumentoEntrada filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion
    }
}