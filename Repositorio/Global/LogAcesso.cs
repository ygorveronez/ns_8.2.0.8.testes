using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class LogAcesso: RepositorioBase<Dominio.Entidades.LogAcesso>, Dominio.Interfaces.Repositorios.LogAcesso
    {
        public LogAcesso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogAcesso> ConsultarRelatorioLogAcesso(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogAcesso filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Repositorio.Global.Consulta.ConsultaLogAcesso().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogAcesso)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogAcesso>();
        }

        public int ContarConsultaRelatorioLogAcesso(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogAcesso filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Repositorio.Global.Consulta.ConsultaLogAcesso().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }
    }
}
