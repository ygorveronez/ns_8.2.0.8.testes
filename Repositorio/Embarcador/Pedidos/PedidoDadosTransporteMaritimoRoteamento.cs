using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoDadosTransporteMaritimoRoteamento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>
    {
        #region Construtores

        public PedidoDadosTransporteMaritimoRoteamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoDadosTransporteMaritimoRoteamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> BuscarPorDadosTransporteMaritimo(int codigoDadosTransporteMaritimo)
        {
            var consultaPedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>()
                .Where(o => o.DadosTransporteMaritimo.Codigo == codigoDadosTransporteMaritimo);

            return consultaPedidoDadosTransporteMaritimo.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento BuscarUltimoPorDadosTransporteMaritimo(int codigoDadosTransporteMaritimo)
        {
            var consultaPedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>()
                .Where(o => o.DadosTransporteMaritimo.Codigo == codigoDadosTransporteMaritimo);

            return consultaPedidoDadosTransporteMaritimo
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> BuscarPorPedido(int codigoPedido)
        {
            var consultaPedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>()
                .Where(o => o.DadosTransporteMaritimo.Pedido.Codigo == codigoPedido);

            return consultaPedidoDadosTransporteMaritimo.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>> BuscarPorPedidoAsync(int codigoPedido)
        {
            var consultaPedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>()
                .Where(o => o.DadosTransporteMaritimo.Pedido.Codigo == codigoPedido);

            return consultaPedidoDadosTransporteMaritimo.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> BuscarPorPedidos(List<int> codigosPedidos)
        {
            var consultaPedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento>()
                .Where(o => codigosPedidos.Contains(o.DadosTransporteMaritimo.Pedido.Codigo));

            return consultaPedidoDadosTransporteMaritimo.ToList();
        }

        #endregion Métodos Públicos
    }
}
