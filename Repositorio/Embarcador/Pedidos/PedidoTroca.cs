using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoTroca : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>
    {
        #region Construtores

        public PedidoTroca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca> BuscarPorPedidoDefinitivo(int codigoPedidoDefinitivo)
        {
            var consultaPedidoTroca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>()
                .Where(o => o.PedidoDefinitivo.Codigo == codigoPedidoDefinitivo);

            consultaPedidoTroca = consultaPedidoTroca
                .Fetch(o => o.PedidoDefinitivo)
                .Fetch(o => o.PedidoProvisorio);

            return consultaPedidoTroca.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca> BuscarPorPedidosDefinitivos(List<int> codigosPedidoDefinitivo)
        {
            var consultaPedidoTroca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>()
                .Where(o => codigosPedidoDefinitivo.Contains(o.PedidoDefinitivo.Codigo));

            consultaPedidoTroca = consultaPedidoTroca
                .Fetch(o => o.PedidoProvisorio);

            return consultaPedidoTroca.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca> BuscarPorPedidoProvisorio(int codigoPedidoProvisorio)
        {
            var consultaPedidoTroca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>()
                .Where(o => o.PedidoProvisorio.Codigo == codigoPedidoProvisorio);

            consultaPedidoTroca = consultaPedidoTroca
                .Fetch(o => o.PedidoDefinitivo)
                .Fetch(o => o.PedidoProvisorio);

            return consultaPedidoTroca.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoTroca BuscarPrimeiroPorPedidoDefinitivo(int codigoPedidoDefinitivo)
        {
            var consultaPedidoTroca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>()
                .Where(o => o.PedidoDefinitivo.Codigo == codigoPedidoDefinitivo);

            return consultaPedidoTroca
                .Fetch(o => o.PedidoProvisorio)
                .FirstOrDefault();
        }

        public bool VerificarExistePorPedidoDefinitivo(int codigoPedidoDefinitivo)
        {
            var consultaPedidoTroca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>()
                .Where(o => o.PedidoDefinitivo.Codigo == codigoPedidoDefinitivo);

            return consultaPedidoTroca.Count() > 0;
        }

        public bool VerificarExistePorPedidoProvisorio(int codigoPedidoProvisorio)
        {
            var consultaPedidoTroca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>()
                .Where(o => o.PedidoProvisorio.Codigo == codigoPedidoProvisorio);

            return consultaPedidoTroca.Count() > 0;
        }

        #endregion
    }
}
