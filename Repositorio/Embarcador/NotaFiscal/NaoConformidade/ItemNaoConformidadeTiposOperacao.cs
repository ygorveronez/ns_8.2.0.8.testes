using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ItemNaoConformidadeTiposOperacao : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao>
    {
        public ItemNaoConformidadeTiposOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao> BuscarPorItensNaoConformidadeAtivos()
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao>()
                .Where(item => item.ItemNaoConformidade.IrrelevanteParaNC == false);

            return consultaItemNaoConformidadeTiposOperacao
                .Select(item => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao()
                {
                    Codigo = item.Codigo,
                    CodigoItemNaoConformidade = item.ItemNaoConformidade.Codigo,
                    CodigoTipoOperacao = item.TipoOperacao.Codigo
                })
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao> BuscarPorItemNaoConformidade(int codigo)
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao>()
                .Where(item => item.ItemNaoConformidade.Codigo == codigo);

            return consultaItemNaoConformidadeTiposOperacao.ToList();
        }



        public void DeletarPorItemNaoConformidade(int codigoINC)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE from ItemNaoConformidadeTipoOperacao WHERE ItemNaoConformidade = :INC_CODIGO")
                                     .SetInt32("INC_CODIGO", codigoINC)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE from ItemNaoConformidadeTipoOperacao WHERE ItemNaoConformidade = :INC_CODIGO")
                                .SetInt32("INC_CODIGO", codigoINC)
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
