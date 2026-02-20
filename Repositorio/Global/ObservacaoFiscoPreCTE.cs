using System;


namespace Repositorio
{
    public class ObservacaoFiscoPreCTE : RepositorioBase<Dominio.Entidades.ObservacaoFiscoPreCTE>
    {
        public ObservacaoFiscoPreCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public void DeletarPorPreCTe(int codigoPreCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ObservacaoFiscoPreCTE obj WHERE obj.preCTE.Codigo = :codigoPreCTe")
                                     .SetInt32("codigoPreCTe", codigoPreCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ObservacaoFiscoPreCTE obj WHERE obj.preCTE.Codigo = :codigoPreCTe")
                                    .SetInt32("codigoPreCTe", codigoPreCTe)
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
