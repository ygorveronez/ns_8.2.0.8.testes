using NHibernate.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMonitoramento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento>
    {
        public ConfiguracaoMonitoramento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento>();

            return query.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento>();

            return await query.FirstOrDefaultAsync();
        }
    }
}
