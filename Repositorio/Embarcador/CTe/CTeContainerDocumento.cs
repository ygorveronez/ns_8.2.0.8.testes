using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeContainerDocumento : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>
    {
        public CTeContainerDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> BuscarPorCTe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            var result = from obj in query where obj.ContainerCTE.CTE.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> BuscarPorContainerCTe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            var result = from obj in query where obj.ContainerCTE.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento BuscarPorCTeChave(int codigo, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento>();
            var result = from obj in query where obj.ContainerCTE.CTE.Codigo == codigo && obj.Chave == chave select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CTeContainerDocumento obj WHERE obj.ContainerCTE.Codigo IN (SELECT p.Codigo FROM ContainerCTE p WHERE p.CTE.Codigo = :codigoCTe)")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CTeContainerDocumento obj WHERE obj.ContainerCTE.Codigo IN (SELECT p.Codigo FROM ContainerCTE p WHERE p.CTE.Codigo = :codigoCTe)")
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
