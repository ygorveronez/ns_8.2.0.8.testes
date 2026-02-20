using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ObservacaoFiscoCTE : RepositorioBase<Dominio.Entidades.ObservacaoFiscoCTE>, Dominio.Interfaces.Repositorios.ObservacaoFiscoCTE
    {
        public ObservacaoFiscoCTE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ObservacaoFiscoCTE BuscarPorCodigoECTe(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ObservacaoFiscoCTE>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTE.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ObservacaoFiscoCTE> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ObservacaoFiscoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe select obj;
            return result.ToList();
        }


        public List<Dominio.Entidades.ObservacaoFiscoCTE> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ObservacaoFiscoCTE>();
            var result = from obj in query where obj.CTE.Codigo == codigoCTe && obj.CTE.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.ObservacaoFiscoCTE BuscarPorCTeEIdentificacao(int codigoCTe, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ObservacaoFiscoCTE>();
            var result = from obj in query
                         where
                            obj.CTE.Codigo == codigoCTe &&
                            obj.Identificador.Equals(descricao)
                         select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ObservacaoFiscoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ObservacaoFiscoCTE obj WHERE obj.CTE.Codigo = :codigoCTe")
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
