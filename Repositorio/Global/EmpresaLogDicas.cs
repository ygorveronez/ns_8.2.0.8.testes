using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class EmpresaLogDicas : RepositorioBase<Dominio.Entidades.EmpresaLogDicas>, Dominio.Interfaces.Repositorios.EmpresaLogDicas
    {
        public EmpresaLogDicas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EmpresaLogDicas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaLogDicas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EmpresaLogDicas> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaLogDicas>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.OrderByDescending(o => o.Data).ToList();
        }
    }
}
