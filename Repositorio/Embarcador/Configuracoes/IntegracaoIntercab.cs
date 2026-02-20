using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoIntercab : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab>
    {
        public IntegracaoIntercab(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoIntercab(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab BuscarIntegracao()
        {
            var consultaIntegracaoIntercab = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab>();
            return consultaIntegracaoIntercab.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab> BuscarIntegracaoAsync()
        {
            var consultaIntegracaoIntercab = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab>();
            return await consultaIntegracaoIntercab.FirstOrDefaultAsync();
        }

        public bool PossuiIntegracaoIntercab()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab>();
            var result = from obj in query where obj.PossuiIntegracaoIntercab select obj;
            return result.Any();
        }

        public async Task<bool> PossuiIntegracaoIntercabAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab>();
            var result = from obj in query where obj.PossuiIntegracaoIntercab select obj;
            return await result.AnyAsync();
        }

    }
}