using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado>
    {
        public ConfiguracaoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoChamado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado>();

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado> BuscarConfiguracaoPadraoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado>();

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public bool PermitirSelecionarCteApenasComNfeVinculadaOcorrenciao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado>();

            return query.Select(o => o.PermitirSelecionarCteApenasComNfeVinculadaOcorrencia).FirstOrDefault();
        }
    }
}
