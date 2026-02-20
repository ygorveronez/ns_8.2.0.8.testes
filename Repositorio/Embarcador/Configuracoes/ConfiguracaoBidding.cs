using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoBidding : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding>
    {
        public ConfiguracaoBidding(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoBidding(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding>();

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding>();

            return query.FirstOrDefaultAsync(CancellationToken);
        }
    }
}