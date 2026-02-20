using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoPortalMultiClifor : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor>
    {
        public ConfiguracaoPortalMultiClifor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor>();

            return query.FirstOrDefault();
        }
    }
}