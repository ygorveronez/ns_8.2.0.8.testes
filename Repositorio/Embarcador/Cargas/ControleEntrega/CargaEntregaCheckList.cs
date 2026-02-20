using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaCheckList : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>
    {
        public CargaEntregaCheckList(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList BuscarPorCargaEntrega(int codigoCargaEntrega, TipoCheckList tipoCheckList = TipoCheckList.Coleta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.TipoCheckList == tipoCheckList);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList> BuscarTodosPorCargaEntrega(int codigoCargaEntrega, TipoCheckList tipoCheckList = TipoCheckList.Coleta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.TipoCheckList == tipoCheckList);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList BuscarPrimeiroPorCarga(int codigoCarga, TipoCheckList tipoCheckList = TipoCheckList.Coleta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga && obj.TipoCheckList == tipoCheckList);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList> BuscarPorCargaEntregaCodigoCarga(int codigoCarga, TipoCheckList tipoCheckList = TipoCheckList.Coleta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga && obj.TipoCheckList == tipoCheckList);
            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChecklist = new ConsultaCargaEntregaChecklist().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaChecklist.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist)));

            return consultaChecklist.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaChecklist = new ConsultaCargaEntregaChecklist().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaChecklist.SetTimeout(600).UniqueResult<int>();
        }
    }
}
