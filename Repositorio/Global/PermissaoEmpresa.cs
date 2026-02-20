using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PermissaoEmpresa : RepositorioBase<Dominio.Entidades.PermissaoEmpresa>, Dominio.Interfaces.Repositorios.PermissaoEmpresa
    {
        public PermissaoEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.PermissaoEmpresa> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PermissaoEmpresa>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.PermissaoEmpresa BuscarPorPaginaEEmpresa(int codigoEmpresa, int codigoPagina)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PermissaoEmpresa>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Pagina.Codigo == codigoPagina select obj;
            return result.FirstOrDefault();
        }
    }
}
