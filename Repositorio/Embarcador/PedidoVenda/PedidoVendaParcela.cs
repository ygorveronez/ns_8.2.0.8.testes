using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class PedidoVendaParcela : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela>
    {
        public PedidoVendaParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela> BuscarPorPedidos(List<int> codigosPedidoVenda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela>();
            var result = from obj in query where codigosPedidoVenda.Contains(obj.PedidoVenda.Codigo) select obj;
            return result.ToList();
        }

        public void DeletarPorPedido(int codigoPedidoVenda)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoVendaParcela obj WHERE obj.PedidoVenda.Codigo = :codigoPedidoVenda")
                                     .SetInt32("codigoPedidoVenda", codigoPedidoVenda)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoVendaParcela obj WHERE obj.PedidoVenda.Codigo = :codigoPedidoVenda")
                                .SetInt32("codigoPedidoVenda", codigoPedidoVenda)
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
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }
}
