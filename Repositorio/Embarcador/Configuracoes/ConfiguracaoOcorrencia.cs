using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia>
    {
        public ConfiguracaoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoOcorrencia(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia>();

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia>();

            return query.FirstOrDefaultAsync(CancellationToken);
        }
    }
}
