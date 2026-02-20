using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class PedidoVendaItens : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens>
    {
        public PedidoVendaItens(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens> BuscarPorPedidoVenda(int codigoPedidoVenda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens>();
            var result = from obj in query where obj.PedidoVenda.Codigo == codigoPedidoVenda select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens BuscarPorPedidoVendaEServico(int codigoPedidoVenda, int codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens>();
            var result = from obj in query where obj.PedidoVenda.Codigo == codigoPedidoVenda && obj.Servico.Codigo == codigoServico select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens> BuscarPorPedidos(List<int> codigosPedidoVenda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens>();
            var result = from obj in query where codigosPedidoVenda.Contains(obj.PedidoVenda.Codigo) select obj;
            return result.ToList();
        }

        public decimal BuscarValorTotal(int codigoPedidoVenca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens>();
            var result = from obj in query where obj.PedidoVenda.Codigo == codigoPedidoVenca select obj;
            return result.Sum(o => (decimal?)o.ValorTotal) ?? 0m;
        }

        public void DeletarPorPedido(int codigoPedidoVenda)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoVendaItens obj WHERE obj.PedidoVenda.Codigo = :codigoPedidoVenda")
                                     .SetInt32("codigoPedidoVenda", codigoPedidoVenda)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoVendaItens obj WHERE obj.PedidoVenda.Codigo = :codigoPedidoVenda")
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
