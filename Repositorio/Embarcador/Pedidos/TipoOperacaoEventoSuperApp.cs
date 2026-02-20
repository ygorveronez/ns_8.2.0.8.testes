using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoEventoSuperApp : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>
    {
        #region Construtores

        public TipoOperacaoEventoSuperApp(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public TipoOperacaoEventoSuperApp(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos Async
        public Task<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> BuscarPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }
        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>()
               .Where(tipoOperacao => codigos.Contains(tipoOperacao.Codigo));

            return query
                .ToListAsync();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>> BuscarPorCodigosEventoSuperAppAsync(List<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> query =
                this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>()
                    .Where(tipoOperacao => codigos.Contains(tipoOperacao.EventoSuperApp.Codigo) && tipoOperacao.TipoOperacao != null);

            return await query.ToListAsync();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>> BuscarPorTipoOperacaoAsync(int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>()
               .Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);

            return query
                .ToListAsync();
        }
        #endregion

        #region Metódos Públicos Sincronos
        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>> BuscarPorTipoOperacao(int tipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> result = from obj in query select obj;
            result = result.Where(ent => ent.TipoOperacao.Codigo == tipoOperacao);

            return result.ToListAsync();
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}