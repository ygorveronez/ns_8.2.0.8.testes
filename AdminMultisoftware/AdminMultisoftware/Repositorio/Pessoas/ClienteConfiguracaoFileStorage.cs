using System.Linq;

namespace AdminMultisoftware.Repositorio.Pessoas
{
    public class ClienteConfiguracaoFileStorage : RepositorioBase<Dominio.Entidades.Pessoas.ClienteConfiguracaoFileStorage>
    {
        public ClienteConfiguracaoFileStorage(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Pessoas.ClienteConfiguracaoFileStorage BuscarPorCliente(int codigoCliente, bool homologacao)
        {
            IQueryable<Dominio.Entidades.Pessoas.ClienteConfiguracaoFileStorage> query = SessionNHiBernate.Query<Dominio.Entidades.Pessoas.ClienteConfiguracaoFileStorage>();

            return query.Where(o => o.Cliente.Codigo == codigoCliente && o.Homologacao == homologacao).FirstOrDefault();
        }
    }
}
