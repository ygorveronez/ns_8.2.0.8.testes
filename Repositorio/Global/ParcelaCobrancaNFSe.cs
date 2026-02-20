using System;

namespace Repositorio
{
    public class ParcelaCobrancaNFSe : RepositorioBase<Dominio.Entidades.ParcelaCobrancaNFSe>, Dominio.Interfaces.Repositorios.ParcelaCobrancaNFSe
    {
        public ParcelaCobrancaNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorNFSe(int codigoNFSe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ParcelaCobrancaNFSe obj WHERE obj.Cobranca.Codigo IN (SELECT cob.Codigo FROM CobrancaNFSe cob WHERE cob.NFSe.Codigo = :codigoNFSe)")
                                     .SetInt32("codigoNFSe", codigoNFSe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ParcelaCobrancaNFSe obj WHERE obj.Cobranca.Codigo IN (SELECT cob.Codigo FROM CobrancaNFSe cob WHERE cob.NFSe.Codigo = :codigoNFSe)")
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
