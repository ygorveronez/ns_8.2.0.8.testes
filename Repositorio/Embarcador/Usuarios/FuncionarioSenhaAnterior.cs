using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Usuarios
{
    public class FuncionarioSenhaAnterior : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior>
    {
        public FuncionarioSenhaAnterior(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior> BuscarPorUsuario(int usuario, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior>();

            var result = from obj in query where obj.Usuario.Codigo == usuario select obj;

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderByDescending(obj => obj.Codigo);

            return result.ToList();
        }
    }
}
