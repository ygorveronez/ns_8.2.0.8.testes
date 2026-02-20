using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Usuarios
{
    public class PerfilFormularioPermissaoPersonalizada : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada>
    {
        public PerfilFormularioPermissaoPersonalizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada> BuscarPorPerfil(int perfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada>();
            var result = from obj in query
                         where obj.PerfilFormulario.PerfilAcesso.Codigo == perfil
                         select obj;
            return result.ToList();
        }
    }
}
