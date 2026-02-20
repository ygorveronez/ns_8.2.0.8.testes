using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class SelecaoFuncionario : RepositorioBase<Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario>
    {
        public SelecaoFuncionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario> BuscarPorSelecao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario>();
            var result = from obj in query where obj.Selecao.Codigo == codigo select obj;
            return result.ToList();
        }

        public void DeletarPorSelecao(int codigo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE SelecaoFuncionario obj WHERE obj.Selecao.Codigo = :codigo")
                                     .SetInt32("codigo", codigo)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE SelecaoFuncionario obj WHERE obj.Selecao.Codigo = :codigo")
                                .SetInt32("codigo", codigo)
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
