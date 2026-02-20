using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoProdutoDivisaoCapacidade : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade>
    {
        #region Construtores

        public CargaPedidoProdutoDivisaoCapacidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaPedidoProdutoDivisaoCapacidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade>()
                .Where(obj => obj.CargaPedidoProduto.CargaPedido.Carga.Codigo == codigoCarga);

            return consultaCargaPedidoProdutoDivisaoCapacidade
                .Fetch(obj => obj.ModeloVeicularCargaDivisaoCapacidade).ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.ModeloVeicularCargaDivisaoCapacidade).ThenFetch(obj => obj.UnidadeMedida)
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> BuscarPorCargasPedidoProduto(List<int> codigos)
        {
            var consultaCargaPedidoProdutoDivisaoCapacidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade>()
                .Where(obj => codigos.Contains(obj.CargaPedidoProduto.Codigo));

            return consultaCargaPedidoProdutoDivisaoCapacidade
                .ToList();
        }

        public void ExcluirTodosPorCargaPedido(int cargaPedido)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoProdutoDivisaoCapacidade obj WHERE obj.CargaPedidoProduto in (select obj.Codigo from CargaPedidoProduto obj where obj.CargaPedido.Codigo =:CargaPedido)")
                             .SetInt32("CargaPedido", cargaPedido)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosPorCargaPedidoProduto(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery("delete CargaPedidoProdutoDivisaoCapacidade divisaoCapacidade WHERE divisaoCapacidade.CargaPedidoProduto.Codigo in (select produto.Codigo from CargaPedidoProduto produto where produto.Codigo =:codigo)")
                        .SetInt32("codigo", codigo)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery("delete CargaPedidoProdutoDivisaoCapacidade divisaoCapacidade WHERE divisaoCapacidade.CargaPedidoProduto.Codigo in (select produto.Codigo from CargaPedidoProduto produto where produto.Codigo =:codigo)")
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
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion
    }
}
