using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public sealed class PedidoCTeParcial : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>
    {
        #region Construtores

        public PedidoCTeParcial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial BuscarPorCodigo(int codigo)
        {
            var consultaPedidoCTeParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>()
                .Where(o => o.Codigo == codigo);

            return consultaPedidoCTeParcial.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> BuscarPorPedido(int codigoPedido)
        {
            var consultaPedidoCTeParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>()
                .Where(o => o.Pedido.Codigo == codigoPedido);

            return consultaPedidoCTeParcial.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> BuscarPorPedidos(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(x => x.Pedido).ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>> BuscarPorPedidosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>();
                var filter = from obj in query
                             where tmp.Contains(obj.Pedido.Codigo)
                             select obj;

                result.AddRange(await filter.Fetch(x => x.Pedido).ToListAsync());

                start += take;
            }

            return result;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial BuscarPorIntegradoOutroPedido(double cpfCnpjRemetente, int numeroCte, int codigoFilial)
        {
            var consultaPedidoCTeParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial>()
                .Where(o => o.Numero == numeroCte && o.Pedido.Remetente.CPF_CNPJ == cpfCnpjRemetente && o.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);

            if (codigoFilial > 0)
                consultaPedidoCTeParcial = consultaPedidoCTeParcial.Where(obj => obj.Pedido.Filial.Codigo == codigoFilial);

            return consultaPedidoCTeParcial.FirstOrDefault();
        }

        #endregion
    }
}
