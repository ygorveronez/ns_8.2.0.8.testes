using System;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class RotaFretePracaPedagio : RepositorioBase<Dominio.Entidades.RotaFretePracaPedagio>
    {
        public RotaFretePracaPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public RotaFretePracaPedagio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.RotaFretePracaPedagio> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFretePracaPedagio>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.RotaFrete.Codigo == codigoRotaFrete);

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.RotaFretePracaPedagio>> BuscarPorRotaFreteAsync(int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFretePracaPedagio>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.RotaFrete.Codigo == codigoRotaFrete);

            return await result.ToListAsync();
        }

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFretePracaPedagio c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RotaFretePracaPedagio c WHERE c.RotaFrete.Codigo = :codigoRotaFrete").SetInt32("codigoRotaFrete", codigoRotaFrete).ExecuteUpdate();

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
