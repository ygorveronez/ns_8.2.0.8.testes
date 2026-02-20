using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public sealed class PedidoNotaParcial : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>
    {
        #region Construtores

        public PedidoNotaParcial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial BuscarPorCodigo(int codigo)
        {
            var consultaPedidoNotaParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>()
                .Where(o => o.Codigo == codigo);

            return consultaPedidoNotaParcial.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> BuscarPorPedido(int codigoPedido)
        {
            var consultaPedidoNotaParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>()
                .Where(o => o.Pedido.Codigo == codigoPedido);

            return consultaPedidoNotaParcial.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> BuscarPorPedidos(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(x => x.Pedido).ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>> BuscarPorPedidosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>();
                var filter = await query
                    .Where(obj => tmp.Contains(obj.Pedido.Codigo))
                    .Fetch(x => x.Pedido).ToListAsync();

                result.AddRange(filter);

                start += take;
            }

            return result;
        }

        public bool BuscarExistePorPedido(int codigoPedido)
        {
            var consultaPedidoNotaParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>()
                .Where(o => o.Pedido.Codigo == codigoPedido);

            return consultaPedidoNotaParcial.FirstOrDefault() != null;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial BuscarPorIntegradaOutroPedido(double cpfCnpjRemetente, int numeroNF, int codigoFilial)
        {
            var consultaPedidoNotaParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>()
                .Where(o => o.Numero == numeroNF && o.Pedido.Remetente.CPF_CNPJ == cpfCnpjRemetente && o.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);

            if (codigoFilial > 0)
                consultaPedidoNotaParcial = consultaPedidoNotaParcial.Where(obj => obj.Pedido.Filial.Codigo == codigoFilial);

            return consultaPedidoNotaParcial.FirstOrDefault();
        }

        public bool PossuiNotaPorPedido(int codigoPedido, int numeroNF)
        {
            var consultaPedidoNotaParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.Numero == numeroNF);

            return consultaPedidoNotaParcial.Any();
        }

        #endregion
    }
}
