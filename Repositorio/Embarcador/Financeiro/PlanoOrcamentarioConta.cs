using System;

namespace Repositorio.Embarcador.Financeiro
{
    public class PlanoOrcamentarioConta : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta>
    {
        public PlanoOrcamentarioConta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorPlanoOrcamentario(int codigoPlanoOrcamentario)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PlanoOrcamentarioConta obj WHERE obj.PlanoOrcamentario.Codigo = :codigoPlanoOrcamentario")
                                     .SetInt32("codigoPlanoOrcamentario", codigoPlanoOrcamentario)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PlanoOrcamentarioConta obj WHERE obj.PlanoOrcamentario.Codigo = :codigoPlanoOrcamentario")
                                .SetInt32("codigoPlanoOrcamentario", codigoPlanoOrcamentario)
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
