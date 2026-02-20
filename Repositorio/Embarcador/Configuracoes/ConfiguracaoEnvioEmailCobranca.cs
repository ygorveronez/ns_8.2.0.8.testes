using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoEnvioEmailCobranca : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca>
    {
        public ConfiguracaoEnvioEmailCobranca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca>();

            return query.FirstOrDefault();
        }
    }
}
