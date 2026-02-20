using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoXMLNotaFiscalTemporario : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario>
    {
        public CargaPedidoXMLNotaFiscalTemporario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario> BuscarPorPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario>();

            query = from obj in query where codigosPedidos.Contains(obj.Pedido.Protocolo) select obj;

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario> BuscarPorCargaEPedido(int codigoCarga, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario>();

            query = from obj in query where obj.Carga.Protocolo == codigoCarga && obj.Pedido.Codigo == codigoPedido select obj;

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        #endregion


    }
}
