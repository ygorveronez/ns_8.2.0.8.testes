using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public class CondicaoPagamentoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.CondicaoPagamentoTransportador>
    {
        #region Construtores

        public CondicaoPagamentoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> BuscarObjetoPorEmpresa(int codigoEmpresa)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.CondicaoPagamentoTransportador>()
                .Where(o => o.Empresa.AtivarCondicao && (o.Empresa.Codigo == codigoEmpresa))
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento
                {
                    CodigoTipoCarga = o.TipoDeCarga.Codigo,
                    CodigoTipoOperacao = o.TipoOperacao.Codigo,
                    DiaEmissaoLimite = o.DiaEmissaoLimite,
                    DiaMes = o.DiaMes,
                    DiasDePrazoPagamento = o.DiasDePrazoPagamento,
                    DiaSemana = o.DiaSemana,
                    TipoPrazoPagamento = o.TipoPrazoPagamento,
                    VencimentoForaMes = o.VencimentoForaMes,
                    ConsiderarDiaUtilVencimento = (bool?)o.ConsiderarDiaUtilVencimento,
                }).OrderBy(obj => obj.DiaEmissaoLimite).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioCondicoesPagamentoTransportador> ConsultarRelatorioCondicoesPagamentoTransportador(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCondicoes = new Repositorio.Embarcador.Financeiro.ConsultaCondicoesPagamentoTransportador().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCondicoes.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioCondicoesPagamentoTransportador)));

            return consultaCondicoes.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioCondicoesPagamentoTransportador>();
        }

        public int ContarConsultaRelatorioCondicoesPagamentoTransportador(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPacotes = new Repositorio.Embarcador.Financeiro.ConsultaCondicoesPagamentoTransportador().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPacotes.SetTimeout(1200).UniqueResult<int>();
        }

        #endregion
    }
}
