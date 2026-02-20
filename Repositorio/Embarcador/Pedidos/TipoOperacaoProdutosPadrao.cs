using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoProdutosPadrao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoProdutosPadrao>
    {
        #region Construtores

        public TipoOperacaoProdutosPadrao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public void DeletarPorTipoOperacao(int codigoTipoOperacao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM TipoOperacaoProdutosPadrao c WHERE c.TipoOperacao.Codigo = :codigoTipoOperacao").SetInt32("codigoTipoOperacao", codigoTipoOperacao).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM TipoOperacaoProdutosPadrao c WHERE c.TipoOperacao.Codigo = :codigoTipoOperacao").SetInt32("codigoTipoOperacao", codigoTipoOperacao).ExecuteUpdate();

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
                throw new Exception("Não foi possível excluir os dados.", ex);
            }
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> BuscarProdutosPorTipoOperacao(int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoProdutosPadrao>();

            var result = from obj in query where obj.TipoOperacao.Codigo == tipoOperacao select obj;

            return result.Select(obj => obj.Produto).ToList();
        }
        #endregion
    }
}
