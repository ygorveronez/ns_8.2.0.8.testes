using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoClienteMultisoftware : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware>
    {
        public ConfiguracaoClienteMultisoftware(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware>();

            return query.FirstOrDefault();
        }
    }
}
