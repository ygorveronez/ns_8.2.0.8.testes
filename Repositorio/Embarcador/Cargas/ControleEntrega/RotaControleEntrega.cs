using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class RotaControleEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>
    {
        public RotaControleEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos - Relatórios
        public int ContarConsultaRelatorioRotaControleEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaRotaControleEntrega = new ConsultaRotaControleEntrega().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaRotaControleEntrega.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RotaControleEntrega> ConsultarRelatorioRotaControleEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRotaControleEntrega = new ConsultaRotaControleEntrega().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaRotaControleEntrega.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RotaControleEntrega)));

            return consultaRotaControleEntrega.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RotaControleEntrega>();
        }
        #endregion


    }
}
