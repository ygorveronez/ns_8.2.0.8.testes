using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Rateio
{
    public class RateioCargaPedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>
    {
        public RateioCargaPedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> buscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Pedido.Codigo == codigoPedido && obj.CargaCTe.CTe.Status == "A"  select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> buscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> BuscarPorProtocoloPedido(int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();

            query = query.Where(o => o.CargaPedido.Pedido.Protocolo == protocoloPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> buscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && (obj.CargaCTe.CTe.Status == "A" || (obj.CargaCTe.CTe == null &&  (obj.CargaNFS != null && obj.CargaNFS.NotaFiscalServico != null)))  select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> buscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();
            var result = from obj in query where obj.CargaCTe.CTe.Codigo == codigoCTe && (obj.CargaCTe.CTe.Status == "A" || (obj.CargaCTe.CTe == null && (obj.CargaNFS != null && obj.CargaNFS.NotaFiscalServico != null))) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> buscarPorNFs(int codigoNFs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();
            var result = from obj in query where obj.CargaNFS.NotaFiscalServico.Codigo == codigoNFs select obj;
            return result.ToList();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && (obj.CargaCTe.CTe.Status == "A" || (obj.CargaCTe.CTe == null && (obj.CargaNFS != null && obj.CargaNFS.NotaFiscalServico != null))) select obj;
            return result.Count();
        }
    }
}
