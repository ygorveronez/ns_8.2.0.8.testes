using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaNotaFiscalItemDevolucao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao>
    {
        #region Construtores
        public CargaEntregaNotaFiscalItemDevolucao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao> BuscarPorCargaEntregaNotaFiscal(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao>();
            var result = query.Where(obj => obj.CargaEntregaNotaFiscal.Codigo == codigo);
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao> BuscarPorCargaEntrega(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao>();
            var result = query.Where(obj => obj.CargaEntregaNotaFiscal.CargaEntrega.Codigo == codigo);
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao> BuscarPorNotaFiscal(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao>();
            var result = query.Where(obj => obj.CargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigo);
            return result.ToList();
        }
    }
}
