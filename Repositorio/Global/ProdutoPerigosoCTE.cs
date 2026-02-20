using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ProdutoPerigosoCTE : RepositorioBase<Dominio.Entidades.ProdutoPerigosoCTE>, Dominio.Interfaces.Repositorios.ProdutoPerigosoCTE
    {
        public ProdutoPerigosoCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ProdutoPerigosoCTE BuscarPorCodigoECTe(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoPerigosoCTE>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTE.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ProdutoPerigosoCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoPerigosoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ProdutoPerigosoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProdutoPerigosoCTE>();
            var result = from obj in query where obj.CTE.Empresa.Codigo == codigoEmpresa && obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ProdutoPerigosoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ProdutoPerigosoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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
