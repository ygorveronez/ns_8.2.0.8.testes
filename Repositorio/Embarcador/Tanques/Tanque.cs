using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Tanques
{
    public class Tanque : RepositorioBase<Dominio.Entidades.Embarcador.Tanques.Tanque>
    {
        public Tanque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Tanques.Tanque BuscarPorID(string id)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Tanques.Tanque>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.ID == id);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Tanques.Tanque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Tanques.Tanque>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }
    }

}

