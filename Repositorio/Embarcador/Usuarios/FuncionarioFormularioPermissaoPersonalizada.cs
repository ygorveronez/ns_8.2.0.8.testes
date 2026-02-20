using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;



namespace Repositorio.Embarcador.Usuarios
{
    public class FuncionarioFormularioPermissaoPersonalizada : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada>
    {
        public FuncionarioFormularioPermissaoPersonalizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada> buscarPorUsuarioFormulario(int usuarioFormulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada>();
            var result = from obj in query where obj.FuncionarioFormulario.Codigo == usuarioFormulario select obj;
            return result.ToList();
        }
    }
}
