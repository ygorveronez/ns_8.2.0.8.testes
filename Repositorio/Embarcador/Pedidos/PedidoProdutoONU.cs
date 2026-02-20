using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoProdutoONU : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU>
    {
        public PedidoProdutoONU(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> BuscarPorPedido(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU>();
            var result = from obj in query where obj.PedidoProduto.Pedido.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> BuscarPorProduto(int codigoProduto)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU>();

            query = query.Where(o => o.PedidoProduto.Codigo == codigoProduto);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU>();
            query = query.Where(p => resultCargaPedido.Contains(p.PedidoProduto.Pedido.Codigo));

            return query.ToList();
        }

        public void DeletarPorPedido(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoProdutoONU obj WHERE obj.PedidoProduto.Pedido.Codigo = :codigo")
                                     .SetInt32("codigo", codigo)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoProdutoONU obj WHERE obj.PedidoProduto.Pedido.Codigo = :codigo")
                                .SetInt32("codigo", codigo)
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
