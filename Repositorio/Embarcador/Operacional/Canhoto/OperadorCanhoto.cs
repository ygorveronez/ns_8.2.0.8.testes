using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Operacional.Canhoto
{
    public class OperadorCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto>
    {
        public OperadorCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto BuscarPorUsuario(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario select obj;
            return result
                .FirstOrDefault();
        }
    }
}
