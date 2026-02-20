using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoChaveCTe : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe>
    {
        public PedidoChaveCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe> BuscarPorPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe>()
                .Where(pa => codigosPedidos.Contains(pa.Pedido.Codigo));

            return query
                .ToList();
        }

        public void DeletarPorPedido(int codigoPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoChaveCTe obj WHERE obj.Pedido.Codigo = :Pedido")
                                     .SetInt32("Pedido", codigoPedido)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoChaveCTe obj WHERE obj.Pedido.Codigo = :Pedido")
                                    .SetInt32("Pedido", codigoPedido)
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
