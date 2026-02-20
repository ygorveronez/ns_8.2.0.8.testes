using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoXMLNotaFiscalTabelaFreteCliente : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente>
    {
        public CargaPedidoXMLNotaFiscalTabelaFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente> BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal, bool tabelaFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal && o.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora);

            return query.FirstOrDefault();
        }

        public void DeletarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoXMLNotaFiscalTabelaFreteCliente obj WHERE obj.PedidoXMLNotaFiscal.Codigo = :codigoPedidoXMLNotaFiscal")
                                 .SetInt32("codigoPedidoXMLNotaFiscal", codigoPedidoXMLNotaFiscal)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoXMLNotaFiscalTabelaFreteCliente obj WHERE obj.PedidoXMLNotaFiscal.Codigo = :codigoPedidoXMLNotaFiscal")
                                .SetInt32("codigoPedidoXMLNotaFiscal", codigoPedidoXMLNotaFiscal)
                                .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }
    }
}
