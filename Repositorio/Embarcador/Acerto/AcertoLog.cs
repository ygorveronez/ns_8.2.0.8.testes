using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoLog : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoLog>
    {
        public AcertoLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoLog BuscarPorCodigoFuncionario(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoLog>();
            var result = from obj in query where obj.Usuario.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
