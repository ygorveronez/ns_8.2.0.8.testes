using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class AlteracaoPedidoProdutoAgendamento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>
    {
        public AlteracaoPedidoProdutoAgendamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>();
            var result = from obj in query where obj.PedidoProduto.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> BuscarPorPedidoNaoVinculado(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>();
            var result = from obj in query where obj.PedidoProduto.Pedido.Codigo == codigoPedido && obj.AgendamentoColeta == null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> BuscarPorAlteracaoPedidosNaoVinculado(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>()
                .Where(x => x.AgendamentoColeta == null && codigosPedidos.Contains(x.PedidoProduto.Pedido.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> BuscarPorCodigoAgendamentoColeta(int codigoAgendamentoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>()
                .Where(obj => obj.AgendamentoColeta.Codigo == codigoAgendamentoColeta);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento BuscarPorCodigoPedidoProduto(int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>();
            var result = from obj in query where obj.PedidoProduto.Codigo == codigoPedidoProduto select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento BuscarPorCodigoPedidoProdutoNaoVinculado(int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>();
            var result = from obj in query where obj.PedidoProduto.Codigo == codigoPedidoProduto && obj.AgendamentoColeta == null select obj;
            return result.FirstOrDefault();
        }
    }
}
