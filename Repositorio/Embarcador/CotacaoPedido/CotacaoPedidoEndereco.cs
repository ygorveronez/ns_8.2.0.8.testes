using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class CotacaoPedidoEndereco : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco>
    {
        public CotacaoPedidoEndereco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
