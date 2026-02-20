using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoComissaoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista>
    {
        public ConfiguracaoComissaoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoComissaoMotorista(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista>();

            return query.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista>();

            return await query.FirstOrDefaultAsync();
        }
    }
}