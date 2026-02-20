using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoIntegracaoEmissorDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento>
    {
        public ConfiguracaoIntegracaoEmissorDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoIntegracaoEmissorDocumento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento>();

            return query
                .Fetch(obj => obj.NSTechIntegradora)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento>();

            return query
                .Fetch(obj => obj.NSTechIntegradora)
                .FirstOrDefaultAsync(CancellationToken);
        }
    }
}