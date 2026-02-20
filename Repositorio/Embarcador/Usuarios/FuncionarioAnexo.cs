using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios
{
    public class FuncionarioAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo>
    {
        public FuncionarioAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> BuscarPorCodigoUsuario(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo>();
            var result = from obj in query where obj.Usuario.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> BuscarAnexosParaFichaMotorista(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo>();
            var result = from obj in query where obj.Usuario.Codigo == codigo && obj.ImprimeNaFichaMotorista select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> BuscarPorCodigoUsuarioETipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoMotorista tipoAnexo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo>();
            var result = from obj in query where (obj.Usuario.Codigo == codigo) && (obj.TipoAnexoMotorista == tipoAnexo) select obj;
            return result.ToList();
        }
    }
}
