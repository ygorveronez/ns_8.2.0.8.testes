using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento>
    {
        public ConfiguracaoFinanceiraAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoFinanceiraAbastecimento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento BuscarPrimeiroRegistro()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento>();

            return query.FirstOrDefault();
        }
    }
}
