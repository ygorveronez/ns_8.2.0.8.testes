using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class EntregaPedidoFoto : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedidoFoto>
    {
        public EntregaPedidoFoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedidoFoto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedidoFoto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}