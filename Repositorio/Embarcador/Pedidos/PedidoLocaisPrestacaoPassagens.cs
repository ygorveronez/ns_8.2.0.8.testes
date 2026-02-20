using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoLocaisPrestacaoPassagens : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>
    {
        public PedidoLocaisPrestacaoPassagens(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public PedidoLocaisPrestacaoPassagens(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens> BuscarPorPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.Pedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool ExistePorPedidos(List<int> codigos)
        {
            var result = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>()
                .Where(obj => codigos.Contains(obj.Pedido.Codigo));

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens BuscarPorPedidoESigla(int codigo, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.Pedido.Codigo == codigo && obj.EstadoDePassagem.Sigla == sigla select obj;
            return result.FirstOrDefault();
        }

    }
}
