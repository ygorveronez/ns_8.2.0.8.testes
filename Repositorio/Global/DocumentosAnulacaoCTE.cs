using System;
using System.Linq;

namespace Repositorio
{
    public class DocumentosAnulacaoCTE : RepositorioBase<Dominio.Entidades.DocumentosAnulacaoCTE>, Dominio.Interfaces.Repositorios.DocumentosAnulacaoCTE
    {
        public DocumentosAnulacaoCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.DocumentosAnulacaoCTE BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosAnulacaoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentosAnulacaoCTE BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosAnulacaoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE DocumentosAnulacaoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE DocumentosAnulacaoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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
