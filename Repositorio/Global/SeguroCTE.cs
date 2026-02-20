using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class SeguroCTE : RepositorioBase<Dominio.Entidades.SeguroCTE>, Dominio.Interfaces.Repositorios.SeguroCTE
    {
        public SeguroCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.SeguroCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SeguroCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.SeguroCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SeguroCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public int ContarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SeguroCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj.Codigo;
            return result.Count();
        }

        public int ContarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SeguroCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.SeguroCTE BuscarPorCTeECodigo(int codigoCTe, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SeguroCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE SeguroCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE SeguroCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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
