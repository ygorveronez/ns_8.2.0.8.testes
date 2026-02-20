using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraPedagio : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio>
    {
        public ConfiguracaoFinanceiraPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio BuscarPrimeiroRegistro()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio>();

            return query.FirstOrDefault();
        }
    }
}
