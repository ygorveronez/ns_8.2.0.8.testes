using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ItemNFSe : RepositorioBase<Dominio.Entidades.ItemNFSe>, Dominio.Interfaces.Repositorios.ItemNFSe
    {
        public ItemNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ItemNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemNFSe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();

        }
        
        public List<Dominio.Entidades.ItemNFSe> BuscarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe select obj;

            return result.ToList();
        }

        public void DeletarPorNFSe(int codigoNFSe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE ItemNFSe obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                     .SetInt32("codigoNFSe", codigoNFSe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE ItemNFSe obj WHERE obj.NFSe.Codigo = :codigoNFSe")
                                    .SetInt32("codigoNFSe", codigoNFSe)
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
