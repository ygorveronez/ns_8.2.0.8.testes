using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.SuperApp
{
    public class ChecklistSuperApp : RepositorioBase<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>
    {
        #region Construtores
        public ChecklistSuperApp(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public ChecklistSuperApp(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Públicos
        public Task<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefaultAsync();
        }

        public Task<List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>()
                .Where(checklist => codigos.Contains(checklist.Codigo));

            return query
                .ToListAsync();
        }

        public Task<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp> BuscarPorIdSuperApp(string idSuperApp)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>()
                .Where(checklist => checklist.IdSuperApp == idSuperApp);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaChecklistSuperApp filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChecklist = Consultar(filtrosPesquisa);

            consultaChecklist = consultaChecklist.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                consultaChecklist = consultaChecklist.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaChecklist = consultaChecklist.Take(parametrosConsulta.LimiteRegistros);

            return consultaChecklist.ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaChecklistSuperApp filtrosPesquisa)
        {
            var consultaPlanejamentoVolume = Consultar(filtrosPesquisa);

            return consultaPlanejamentoVolume.CountAsync();
        }

        private IQueryable<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaChecklistSuperApp filtrosPesquisa)
        {
            var consultaChecklistSuperApp = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp>();
            var result = from obj in consultaChecklistSuperApp select obj;

            if (filtrosPesquisa.Codigo > 0)
                result = result.Where(x => x.Codigo == filtrosPesquisa.Codigo);

            if (!string.IsNullOrEmpty(filtrosPesquisa.IdSuperApp))
                result = result.Where(x => x.IdSuperApp == filtrosPesquisa.IdSuperApp);

            if (!string.IsNullOrEmpty(filtrosPesquisa.Titulo))
                result = result.Where(x => x.Titulo == filtrosPesquisa.Titulo);

            if (filtrosPesquisa.TipoFluxo > 0)
                result = result.Where(x => (int)x.TipoFluxo == filtrosPesquisa.TipoFluxo);

            return result;
        }
        #endregion
    }
}
