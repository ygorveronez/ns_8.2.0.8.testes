using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class NotaFiscalMiro : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro>
    {
        #region Construtor
        public NotaFiscalMiro(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro BuscarPorPedidoXmlNotaFiscal(int codigoPedidoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro>();
            query = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoNotaFiscal select obj;
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro BuscarPorXmlNotaFiscal(int codigoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro>();
            query = from obj in query where obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal select obj;
            return query.FirstOrDefault();
        }
        #endregion
    }
}
