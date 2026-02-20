using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Usuarios
{
    public class PerfilFormulario : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario>
    {
        public PerfilFormulario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> BuscarPorPerfil(int perfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario>();
            var result = from obj in query where obj.PerfilAcesso.Codigo == perfil select obj;

            return result.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario> BuscarPorPerfis(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario>()
                .Where(obj => codigos.Contains(obj.PerfilAcesso.Codigo));
            
            return query
                .ToList();
        }
    }
}
