using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTrafegus : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus>
    {
        public IntegracaoTrafegus(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoTrafegus(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus> BuscarAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus>();

            return query.FirstOrDefaultAsync(CancellationToken);
        }
    }
}
