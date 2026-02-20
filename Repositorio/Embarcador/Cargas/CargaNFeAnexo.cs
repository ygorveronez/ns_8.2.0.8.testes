using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaNFeAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo>
    {
        public CargaNFeAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaNFeAnexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo>> BuscarPorCargaAsync(int codigoCarga, bool isTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);

            if (isTransportador)
                query = query.Where(o => !o.OcultarParaTransportador);

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);

            return query.ToList();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);

            return query.Count();
        }
    }
}
