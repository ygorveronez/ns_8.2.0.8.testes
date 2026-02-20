using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Usuarios
{
    public class FuncionarioFormulario : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario>
    {
        public FuncionarioFormulario(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> buscarPorUsuario(int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario>();
            var result = from obj in query where obj.Usuario.Codigo == usuario select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario BuscarPorUsuarioEFormulario(int codigoUsuario, int codigoFormulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && obj.CodigoFormulario == codigoFormulario select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorUsuarioEFormulario(int codigoUsuario, int codigoFormulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && obj.CodigoFormulario == codigoFormulario select obj;
            return result.Count();
        }

        public int DeletarFormulariosVinculados(int codigoUsuario)
        {
            var sql = $@"delete T_FUNCIONARIO_FORMULARIO WHERE FUN_CODIGO = ${codigoUsuario}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
    }
}
