using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoTabelaFreteCliente : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>
    {
        public CargaPedidoTabelaFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPedidoTabelaFreteCliente(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> BuscarPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> BuscarPorCarga(int cargaPedido, bool tabelaFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == cargaPedido && obj.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente BuscarPorCargaPedido(int cargaPedido, bool tabelaFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> BuscarPorCargaPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool tabelaFreteFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();

            query = query.Where(o => cargaPedidos.Contains(o.CargaPedido) && o.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> BuscarPorCargaPedido(List<int> codigosCargaPedido, bool tabelaFreteFilialEmissora)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();

            int take = 500;
            int start = 0;
            while (start < codigosCargaPedido?.Count)
            {
                List<int> tmp = codigosCargaPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>();
                var filter = from obj in query
                             where tmp.Contains(obj.CargaPedido.Codigo)
                                    && obj.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora
                             select obj;

                result.AddRange(filter.Fetch(o => o.TabelaFreteCliente)
                .Fetch(o => o.CargaPedido)
                .ThenFetch(o => o.Pedido)
                .ThenFetch(o => o.Destinatario)
                .ThenFetch(o => o.Localidade)
                .ThenFetch(o => o.Pais)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Destino)
                .ThenFetch(obj => obj.Pais)
                .ToList());

                start += take;
            }

            return result;
        }


        #endregion
    }
}
