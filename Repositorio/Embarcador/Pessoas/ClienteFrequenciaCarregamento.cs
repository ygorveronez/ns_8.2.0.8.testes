using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteFrequenciaCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento>
    {
        public ClienteFrequenciaCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> BuscarPorCliente(double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento>();

            query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjCliente);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> BuscarPorCliente(List<double>cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento>();

            var result = from obj in query where cpfCnpjCliente.Contains(obj.Cliente.CPF_CNPJ) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> BuscarPorClienteETransportador(double cpfCnpjCliente, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento>();

            query = query.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjCliente && o.Empresa.Codigo == codigoEmpresa);

            return query.ToList();
        }
    }
}
