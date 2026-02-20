using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoXMLNotaFiscalNFS : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS>
    {
        public CargaPedidoXMLNotaFiscalNFS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPorCargaNFS(int codigoCargaNFS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true
                             && obj.CargaNFS.Codigo == codigoCargaNFS
                         select obj.PedidoXMLNotaFiscal.CargaPedido;


            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> BuscarNFSsPorCargaPedido(int cargaPedido, int carga, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS>();

            if (carga > 0)
                query = query.Where(obj => obj.CargaNFS.Carga.Codigo == carga && obj.CargaNFS.NotaFiscalServico.NFSe != null && obj.CargaNFS.NotaFiscalServico.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            var result = query.OrderBy(obj => obj.CargaNFS.Codigo).Select(obj => obj.CargaNFS);
            return result.Distinct().Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS> BuscarCargaPedidoXMLNotaFiscalNFS(int cargaNFS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS>();
            query = query.Where(obj => obj.CargaNFS.Codigo == cargaNFS);

            return query.ToList();
        }



        public int ContarNFSsPorCargaPedido(int cargaPedido, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS>();

            if (carga > 0)
                query = query.Where(obj => obj.CargaNFS.Carga.Codigo == carga && obj.CargaNFS.NotaFiscalServico.NFSe != null && obj.CargaNFS.NotaFiscalServico.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            var result = query.OrderBy(obj => obj.CargaNFS.Codigo).Select(obj => obj.CargaNFS);
            return result.Distinct().Count();
        }

    }
}
