using System;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class CargaPracaPedagio : RepositorioBase<Dominio.Entidades.CargaPracaPedagio>
    {
        public CargaPracaPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPracaPedagio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.CargaPracaPedagio> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaPracaPedagio>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == codigoCarga);

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.CargaPracaPedagio>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaPracaPedagio>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == codigoCarga);

            return await result.ToListAsync();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPracaPedagio c WHERE c.Carga.Codigo = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPracaPedagio c WHERE c.Carga.Codigo = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();

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
        public async Task DeletarPorCargaAsync(int codigoCarga)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPracaPedagio c WHERE c.Carga.Codigo = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        await UnitOfWork.StartAsync();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaPracaPedagio c WHERE c.Carga.Codigo = :codigoCarga").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();

                        await UnitOfWork.CommitChangesAsync();
                    }
                    catch
                    {
                        await UnitOfWork.RollbackAsync();
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
