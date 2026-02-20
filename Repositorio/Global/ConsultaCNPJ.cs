using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ConsultaCNPJ : RepositorioBase<Dominio.Entidades.ConsultaCNPJ>
    {
        public ConsultaCNPJ(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConsultaCNPJ Consultar(string cnpj, string estado, Dominio.Enumeradores.StatusConsultaCNPJ? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConsultaCNPJ>();
            var result = from obj in query select obj;

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(cnpj))
                result = result.Where(o => o.CNPJ == cnpj);

            if (!string.IsNullOrWhiteSpace(estado))
                result = result.Where(o => o.Estado == estado);

            return result.OrderByDescending(o => o.Codigo). FirstOrDefault();
        }

    }
}
