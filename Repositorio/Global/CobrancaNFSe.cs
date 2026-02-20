using System;

namespace Repositorio
{
    public class CobrancaNFSe : RepositorioBase<Dominio.Entidades.CobrancaNFSe>, Dominio.Interfaces.Repositorios.CobrancaNFSe
    {
        public CobrancaNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorNFSe(int codigoNFSe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CobrancaNFSe obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                     .SetInt32("codigoNFSe", codigoNFSe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CobrancaNFSe obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                    .SetInt32("codigoNFSe", codigoNFSe)
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
