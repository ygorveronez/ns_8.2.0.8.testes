using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.SuperApp
{
    public class EventoSuperApp : RepositorioBase<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp>
    {
        #region Construtores
        public EventoSuperApp(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public EventoSuperApp(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Públicos
        public Task<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }
        public Task<List<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp>> BuscarPorCodigoChecklistSuperAppAsync(int codigo)
        {
            return this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp>()
                .Where(e => e.ChecklistSuperApp.Codigo == codigo)
                .ToListAsync(CancellationToken);
        }
        public Task<List<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp>()
                .Where(evento => codigos.Contains(evento.Codigo));

            return query
                .ToListAsync();
        }
        #endregion
    }
}
