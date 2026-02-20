using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class RegrasCotacaoPedidoValor : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor>
    {
        public RegrasCotacaoPedidoValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor>();
            var result = from obj in query where obj.RegrasCotacaoPedido.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
