using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class NFSeItem : RepositorioBase<Dominio.Entidades.NFSeItem>
    {
        public NFSeItem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NFSeItem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeItem>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.NFSeItem> BuscarPorCTe(int codigoCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeItem>();

            var result = from obj in query where obj.CTe.Codigo == codigoCte select obj;

            return result.Fetch(obj => obj.Servico).ToList();
        }

        public Dominio.Entidades.NFSeItem BuscarPrimeiroPorCTe(int codigoCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeItem>();

            var result = from obj in query where obj.CTe.Codigo == codigoCte select obj;

            return result.Fetch(obj => obj.Servico).FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCte)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE NFSeItem obj WHERE obj.CTe.Codigo = :codigoCte")
                                     .SetInt32("codigoCte", codigoCte)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE NFSeItem obj WHERE obj.CTe.Codigo = :codigoCte")
                                    .SetInt32("codigoCte", codigoCte)
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