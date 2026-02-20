using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class MotoristaCTE : RepositorioBase<Dominio.Entidades.MotoristaCTE>, Dominio.Interfaces.Repositorios.MotoristaCTE
    {
        public MotoristaCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.MotoristaCTE BuscarPorCodigoECTe(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.MotoristaCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }


        public List<Dominio.Entidades.MotoristaCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.MotoristaCTE> BuscarPorCTe(int codigoEmpresa, int[] codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();
            var result = from obj in query where codigosCTes.Contains(obj.CTE.Codigo) && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public int ContarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.Count();
        }

        public int ContarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.Count();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE MotoristaCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE MotoristaCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE MotoristaCTE obj WHERE obj.CTE IN (SELECT cargaPedidoDocumentoCTe.CTe FROM CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe WHERE cargaPedidoDocumentoCTe.CargaPedido.Codigo = :codigoCargaPedido)")
                                     .SetInt32("codigoCargaPedido", codigoCargaPedido)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE MotoristaCTE obj WHERE obj.CTE IN (SELECT cargaPedidoDocumentoCTe.CTe FROM CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe WHERE cargaPedidoDocumentoCTe.CargaPedido.Codigo = :codigoCargaPedido)")
                                    .SetInt32("codigoCargaPedido", codigoCargaPedido)
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
