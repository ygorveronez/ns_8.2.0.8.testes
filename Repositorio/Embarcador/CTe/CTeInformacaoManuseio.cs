using System;

namespace Repositorio.Embarcador.CTe
{
    public class CTeInformacaoManuseio : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeInformacaoManuseio>
    {
        public CTeInformacaoManuseio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeInformacaoManuseio obj WHERE obj.ConhecimentoDeTransporteEletronico.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeInformacaoManuseio obj WHERE obj.ConhecimentoDeTransporteEletronico.Codigo = :codigoCTe")
                                    .SetInt32("codigoCTe", codigoCTe)
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
