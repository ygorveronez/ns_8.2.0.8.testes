using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class DevolucaoMotivo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DevolucaoMotivo>
    {
        public DevolucaoMotivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DevolucaoMotivo BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DevolucaoMotivo>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }



    }
}
