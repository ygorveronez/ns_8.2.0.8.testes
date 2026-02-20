using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.FaturamentoMensal
{
    public class PlanoEmissaoNFeValor : RepositorioBase<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor>
    {
        public PlanoEmissaoNFeValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorPlanoEmissao(int codigoPlanoEmissaoNFe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE PlanoEmissaoNFeValor obj WHERE obj.PlanoEmissaoNFe.Codigo = :codigoPlanoEmissaoNFe")
                                     .SetInt32("codigoPlanoEmissaoNFe", codigoPlanoEmissaoNFe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE PlanoEmissaoNFeValor obj WHERE obj.PlanoEmissaoNFe.Codigo = :codigoPlanoEmissaoNFe")
                                .SetInt32("codigoPlanoEmissaoNFe", codigoPlanoEmissaoNFe)
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
