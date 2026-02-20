using System.Linq;

namespace Repositorio
{
    public class ConfiguracaoIntegracaoLsTranslogClientes : RepositorioBase<Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes>
    {
        public ConfiguracaoIntegracaoLsTranslogClientes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

    }
}

