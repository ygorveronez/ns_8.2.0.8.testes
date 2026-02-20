using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class DadosCliente : RepositorioBase<Dominio.Entidades.DadosCliente>, Dominio.Interfaces.Repositorios.DadosCliente
    {
        public DadosCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DadosCliente Buscar(int codigoEmpresa, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DadosCliente>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Cliente.CPF_CNPJ == cpfCnpjCliente select obj;

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.DadosCliente BuscarPrimeiro(double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DadosCliente>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpjCliente select obj;

            return result.FirstOrDefault();
        }
    }
}
