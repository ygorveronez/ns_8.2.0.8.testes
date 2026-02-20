using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoGeral : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral>
    {
        public ConfiguracaoGeral(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoGeral(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral>();

            return query.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral>();
            
            return await query.FirstOrDefaultAsync();
        }
    }
}
