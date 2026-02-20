using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class LogEnvioSMS : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.LogEnvioSMS>
    {
        public LogEnvioSMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioSMS>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            var consulta = new ConsultaLogEnvioSMS().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioSMS)));

            return await consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioSMS>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaLogEnvioSMS().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
    }
}
