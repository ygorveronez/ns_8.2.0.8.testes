using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido>
    {
        public ConfiguracaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoPedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido>();

            return query.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido>();
            return await query.FirstOrDefaultAsync();
        }
    }
}
