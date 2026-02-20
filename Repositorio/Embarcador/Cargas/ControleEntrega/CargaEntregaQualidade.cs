using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaQualidade : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade>
    {
        public CargaEntregaQualidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaEntregaQualidade(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade> BuscarPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade>().Where(obj => obj.Codigo == codigo);
            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade> BuscarPorCodigoCargaEntregaAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            query = query.Where(obj => obj.Codigo == codigo);
            return query.Select(entrega => entrega.CargaEntregaQualidade).FirstOrDefaultAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade> BuscarPorCodigoCargaEntregaAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            query = query.Where(obj => codigos.Contains(obj.Codigo));
            return query.Select(entrega => entrega.CargaEntregaQualidade).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade BuscarPorCodigosCargaEntrega(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            query = query.Where(obj => codigos.Contains(obj.Codigo));
            return query.Select(entrega => entrega.CargaEntregaQualidade).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade BuscarPorCodigoCargaEntrega(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            query = query.Where(obj => obj.Codigo == codigo);
            return query.Select(entrega => entrega.CargaEntregaQualidade).FirstOrDefault();
        }


        #endregion
    }
}