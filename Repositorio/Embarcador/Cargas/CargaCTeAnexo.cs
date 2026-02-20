using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeAnexo>
    {
        public CargaCTeAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaCTeAnexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTeAnexo>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);


            return query.ToListAsync(CancellationToken);
        }

        public Task<int> ContarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCarga);

            return query.CountAsync();
        }
    }
}
