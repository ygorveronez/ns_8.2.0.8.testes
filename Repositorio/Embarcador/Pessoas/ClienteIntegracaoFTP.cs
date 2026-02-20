using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteIntegracaoFTP : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP>
    {
        public ClienteIntegracaoFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ClienteIntegracaoFTP(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Task<Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP> BuscarPorClienteAsync(double cpfCnpjCliente, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteIntegracaoFTP>();
            query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjCliente);
            return query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
