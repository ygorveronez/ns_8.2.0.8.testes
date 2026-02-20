using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>
    {
        public PedidoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> BuscarPorPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();

            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> BuscarPorPedidos(List<int> codigos)
        {          

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();
                var filter = from obj in query
                             where tmp.Contains(obj.EntidadeAnexo.Codigo)
                             select obj;

                result.AddRange(filter.ToList());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>> BuscarPorPedidosAsync(List<int> codigos)
        {

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();

            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();
                var filter = from obj in query
                             where tmp.Contains(obj.EntidadeAnexo.Codigo)
                             select obj;

                result.AddRange(await filter.ToListAsync());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();
            query = query.Where(p => resultCargaPedido.Contains(p.EntidadeAnexo.Codigo));

            return query.ToList();
        }
    }
}
