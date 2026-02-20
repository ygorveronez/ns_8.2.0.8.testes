using System;
using System.Linq;

namespace Repositorio
{
    public class CobrancaCTe : RepositorioBase<Dominio.Entidades.CobrancaCTe>, Dominio.Interfaces.Repositorios.CobrancaCTe
    {
        public CobrancaCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int BuscarUltimoNumero(int codigoEmpresa)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.CobrancaCTe>();
            criteria.CreateAlias("CTe", "cte");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("cte.Empresa.Codigo", codigoEmpresa));
            criteria.SetProjection(NHibernate.Criterion.Projections.Max("Numero"));
            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.CobrancaCTe BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CobrancaCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CobrancaCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CobrancaCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
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
