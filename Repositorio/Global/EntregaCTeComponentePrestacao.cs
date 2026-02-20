using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EntregaCTeComponentePrestacao : RepositorioBase<Dominio.Entidades.EntregaCTeComponentePrestacao>
    {
        public EntregaCTeComponentePrestacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EntregaCTeComponentePrestacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EntregaCTeComponentePrestacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EntregaCTeComponentePrestacao> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EntregaCTeComponentePrestacao>()
                .Where(obj => obj.EntregaCTe.CTE.Codigo == codigoCTe);

            return query.ToList();
        }

        public void DeletarPorEntrega(int codigoEntrega)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE EntregaCTeComponentePrestacao obj WHERE obj.EntregaCTe.Codigo = :codigoEntrega")
                                     .SetInt32("codigoEntrega", codigoEntrega)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE EntregaCTeComponentePrestacao obj WHERE obj.EntregaCTe.Codigo = :codigoEntrega")
                                    .SetInt32("codigoEntrega", codigoEntrega)
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