using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class RegrasCotacaoPedidoTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao>
    {
        public RegrasCotacaoPedidoTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao>();
            var result = from obj in query where obj.RegrasCotacaoPedido.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
