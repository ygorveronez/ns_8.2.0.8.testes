using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaViagemEventos : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento>
    {
        #region Construtores

        public CargaViagemEventos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaViagemEventos> ConsultarRelatorioCargaViagemEventos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaCargaViagemEventos().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaViagemEventos)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaViagemEventos>();
        }

        public int ContarConsultarRelatorioCargaViagemEventos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaCargaViagemEventos().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

    }
}
