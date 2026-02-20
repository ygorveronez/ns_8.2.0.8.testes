using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoWebService : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService>
    {
        public ConfiguracaoWebService(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoWebService(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService BuscarConfiguracaoPadrao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWebService = null)
        {
            return configWebService == null ? SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService>().FirstOrDefault() : configWebService;
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService> BuscarConfiguracaoPadraoAsync(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWebService = null)
        {
            return configWebService == null ? await SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService>().FirstOrDefaultAsync() : configWebService;
        }

        public bool BuscarGerarLogMetodosREST()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService>();

            return query.Select(x => (bool?)x.GerarLogMetodosREST).FirstOrDefault() ?? false;
        }
    }
}
