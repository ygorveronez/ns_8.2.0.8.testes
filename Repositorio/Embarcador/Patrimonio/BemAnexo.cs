using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
    public class BemAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.BemAnexo>
    {
        public BemAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Patrimonio.BemAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
