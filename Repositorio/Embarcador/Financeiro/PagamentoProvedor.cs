using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    public class PagamentoProvedor : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>
    {
        #region Construtores

        public PagamentoProvedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor ObterPagamentoProvedor(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentosProvedorCarga)
        {
            if (pagamentosProvedorCarga == null || !pagamentosProvedorCarga.Any())
                return null;

            if (pagamentosProvedorCarga != null && pagamentosProvedorCarga.Count > 1)
            {
                return pagamentosProvedorCarga
                    .Select(o => o.PagamentoProvedor)
                    .FirstOrDefault(p => p.SituacaoLiberacaoPagamentoProvedor != SituacaoLiberacaoPagamentoProvedor.Rejeitada);
            }

            return pagamentosProvedorCarga
                .Select(o => o.PagamentoProvedor)
                .FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioLiberacaoPagamentoProvedor> ConsultarRelatorioLiberacaoPagamentoProvedor(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPacotes = new Repositorio.Embarcador.Financeiro.ConsultaLiberacaoPagamentoProvedor().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPacotes.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioLiberacaoPagamentoProvedor)));

            return consultaPacotes.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioLiberacaoPagamentoProvedor>();
        }

        public int ContarConsultaRelatorioLiberacaoPagamentoProvedor(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPacotes = new Repositorio.Embarcador.Financeiro.ConsultaLiberacaoPagamentoProvedor().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPacotes.SetTimeout(1200).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
