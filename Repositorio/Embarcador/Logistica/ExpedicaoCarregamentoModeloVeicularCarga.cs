using System;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ExpedicaoCarregamentoModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga>
    {
        #region Construtores

        public ExpedicaoCarregamentoModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public void DeletarPorControleExpedicao(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery($"delete ExpedicaoCarregamentoModeloVeicularCarga modeloVeicularCargaExclusivo where modeloVeicularCargaExclusivo.ExpedicaoCarregamento.Codigo = :codigo ")
                        .SetInt32("codigo", codigo)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery($"delete ExpedicaoCarregamentoModeloVeicularCarga modeloVeicularCargaExclusivo where modeloVeicularCargaExclusivo.ExpedicaoCarregamento.Codigo = :codigo ")
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
