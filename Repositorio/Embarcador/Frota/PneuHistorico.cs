using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public class PneuHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.PneuHistorico>
    {
        #region Construtores

        public PneuHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PneuHistorico(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneuHistorico = new ConsultaPneuHistorico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPneuHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico)));

            return consultaPneuHistorico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneuHistorico = new ConsultaPneuHistorico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPneuHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico)));

            return await consultaPneuHistorico.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuHistorico>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPneuHistorico = new ConsultaPneuHistorico().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPneuHistorico.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaMovimentacoesPneusPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.PneuHistorico>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frota.PneuHistorico> ConsultaMovimentacoesPneusPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.PneuHistorico>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            var teste  = query.ToList();
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
        #endregion
    }
}
