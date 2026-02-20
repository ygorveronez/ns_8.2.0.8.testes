using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoEspelhoIntercement : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>
    {
        public PedidoEspelhoIntercement(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement> BuscarPorCargaPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public bool ContemPorEspelhoCargaPedido(int codigoEspelho, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.EspelhoIntercement.Codigo == codigoEspelho select obj;
            return result.Any();
        }

        public bool ContemNumeroRemessaPorCargaPedido(string numeroRemessa, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.EspelhoIntercement.VBELN == numeroRemessa select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement BuscarPorNumeroRemessaPorCargaPedido(string numeroRemessa, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.EspelhoIntercement.VBELN == numeroRemessa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement BuscarPorNumeroRemessa(string numeroRemessa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.CargaPedido.Carga.SituacaoCarga == situacaoCarga && obj.EspelhoIntercement.VBELN == numeroRemessa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement> ConsultarPorCargaPedido(int codigoCargaPedido, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.Select(c => c.EspelhoIntercement).OrderBy(propOrdenacao + " " + dirOrdenacao)
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .ToList();
        }

        public int ContarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.Select(c => c.EspelhoIntercement).Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement> ConsultarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);
            return query.Select(c => c.EspelhoIntercement).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement> BuscarPorEspelho(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>();
            var result = from obj in query where obj.EspelhoIntercement.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
