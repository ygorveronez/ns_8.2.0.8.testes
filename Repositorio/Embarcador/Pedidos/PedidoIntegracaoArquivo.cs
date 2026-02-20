using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoIntegracaoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo>
    {
        public PedidoIntegracaoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
