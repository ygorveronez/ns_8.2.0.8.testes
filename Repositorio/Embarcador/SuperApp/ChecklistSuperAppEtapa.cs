using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.SuperApp
{
    public class ChecklistSuperAppEtapa : RepositorioBase<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>
    {
        #region Construtores
        public ChecklistSuperAppEtapa(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ChecklistSuperAppEtapa(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Métodos Públicos

        public Task<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>()
               .Where(checklistSuperAppEtapa => codigos.Contains(checklistSuperAppEtapa.Codigo));

            return query
                .ToListAsync();
        }
        public Task<List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>> BuscarPorChecklistAsync(int codigoChecklist)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>()
               .Where(obj => obj.ChecklistSuperApp.Codigo == codigoChecklist);

            return query
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> BuscarPorChecklists(List<int> checklists)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa>()
                .Where(n => checklists.Contains(n.ChecklistSuperApp.Codigo));

            return query.ToList();
        }
        #endregion
    }
}
