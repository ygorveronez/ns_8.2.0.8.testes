using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PerfilPermissaoPagina : RepositorioBase<Dominio.Entidades.PerfilPermissaoPagina >, Dominio.Interfaces.Repositorios.PerfilPermissaoPagina
    {
        public PerfilPermissaoPagina(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PerfilPermissaoPagina BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissaoPagina>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PerfilPermissaoPagina> BuscarPorCodigoPerfil(int codigoPerfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissaoPagina>();
            var result = from obj in query where obj.PerfilPermissao.Codigo == codigoPerfil select obj;
            return result.ToList();
        }

        public Dominio.Entidades.PerfilPermissaoPagina BuscarPorPerfilEPagina(int codigoPerfil, int codigoPagina)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissaoPagina>();
            var result = from obj in query where obj.PerfilPermissao.Codigo == codigoPerfil && obj.Pagina.Codigo == codigoPagina select obj;
            return result.FirstOrDefault();
        }

    }
}
