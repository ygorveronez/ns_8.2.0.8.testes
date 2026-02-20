using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class CotacaoPedidoComponente : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente>
    {
        public CotacaoPedidoComponente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> BuscarPorCotacao(long codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacao select obj;
            return result.ToList();
        }
    }
}
