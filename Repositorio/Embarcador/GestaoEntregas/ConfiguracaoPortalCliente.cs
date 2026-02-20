using System.Linq;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class ConfiguracaoPortalCliente : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente>
    {
        public ConfiguracaoPortalCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente>();

            return query.FirstOrDefault();
        }
    }
}
