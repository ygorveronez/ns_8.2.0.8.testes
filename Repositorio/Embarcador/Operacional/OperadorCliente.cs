using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Operacional
{
    public class OperadorCliente : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.OperadorCliente>
    {
        public OperadorCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Operacional.OperadorCliente> BuscarPorOperador(int codigoOperador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorCliente>();
            var result = from obj in query where obj.OperadorLogistica.Codigo == codigoOperador select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Operacional.OperadorCliente BuscarPorOperadorECliente(int codigo, double cpfcnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorCliente>()
                .Where(obj => obj.OperadorLogistica.Codigo == codigo && obj.Cliente.CPF_CNPJ == cpfcnpjCliente);

            return query.FirstOrDefault();
        }

    }
}
