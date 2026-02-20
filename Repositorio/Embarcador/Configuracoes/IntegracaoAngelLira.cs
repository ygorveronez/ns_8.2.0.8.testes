using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoAngelLira : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira>
    {
        public IntegracaoAngelLira(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoAngelLira(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira Buscar()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira>();

            return query.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira> BuscarAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira>();

            return await query.FirstOrDefaultAsync();
        }
    }
}
