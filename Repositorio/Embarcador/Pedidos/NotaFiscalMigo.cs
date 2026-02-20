using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class NotaFiscalMigo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo>
    {
        #region Construtor
        public NotaFiscalMigo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo BuscarPorPedidoXmlNotaFiscal(int codigoPedidoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo>();
            query = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoNotaFiscal select obj;
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo BuscarPorXmlNotaFiscal(int codigoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo>();
            query = from obj in query where obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal select obj;
            return query.FirstOrDefault();
        }
        #endregion
    }
}
