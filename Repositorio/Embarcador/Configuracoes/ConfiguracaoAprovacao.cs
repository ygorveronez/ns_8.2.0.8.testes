using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAprovacao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao>
    {
        public ConfiguracaoAprovacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoAprovacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao>();

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao>();

            return query.FirstOrDefaultAsync(CancellationToken);
        }
    }
}