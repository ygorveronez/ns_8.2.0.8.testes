using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.TorreControle
{
    public class ConfiguracaoQualidadeEntrega : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega>
    {
        public ConfiguracaoQualidadeEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoQualidadeEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega>();
            return query.FirstOrDefaultAsync(CancellationToken);
        }
    }
}
